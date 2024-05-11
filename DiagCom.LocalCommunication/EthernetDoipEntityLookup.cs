using DiagCom.Doip.Exceptions;
using DiagCom.Doip.Messages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DiagCom.LocalCommunication
{
    public class EthernetDoipEntityLookup
    {
        private readonly ILogger<EthernetDoipEntityLookup> _logger;
        public EthernetDoipEntityLookup(ILogger<EthernetDoipEntityLookup> logger)
        {
            _logger = logger;
        }


        public async Task<List<DoipEntity>> FindAll()
        {
            // Currently, this will give you all ethernet and loopback NICs, i.doipEntities. all vehicles and all local simulators.
            // Production code will not want to examine loopbacks unless "simulator use" is explicitly stated.
            var nics = GetOperationalNics();

            var listenTasks = new List<Task<List<DoipEntity>>>();

            // Testing with real vehicles we have seen that they do not use the ISO 13400-2 stipulated random(0..500) ms before responding
            // to UDP VehicleIdentificationRequest. This leads to lost responses when we have multiple vehicles connected via the same NIC.
            // Therefore we have given up on the strategy to send such requests. This code is outcommented below.


            foreach (var nic in nics)
            {
                var address = GetSingleNicAddress(nic);
                if (address == null)
                {
                    _logger.LogDebug($"No local address assigned for NIC \"{nic.Name}\" yet.");
                    continue;
                }

                listenTasks.Add(ListenForVehicleIdentificationResponses(address));
            }

            var foundDoipEntities = await Task.WhenAll(listenTasks);

            //Convert to VehicleIdentificationMessage
            return foundDoipEntities.SelectMany(x => x).ToList();
        }

        private async Task<List<DoipEntity>> ListenForVehicleIdentificationResponses(IPAddress address)
        {
            const int TimeToWaitForResponseMs = 3000;
            var stopwatch = Stopwatch.StartNew();
            var doipEntities = new List<DoipEntity>();
            try
            {
                using (var udpClient13400 = new UdpClient())
                {
                    var localEndPoint13400 = new IPEndPoint(address, 13400);
                    udpClient13400.Client.Bind(localEndPoint13400);
                    while (stopwatch.ElapsedMilliseconds < TimeToWaitForResponseMs)
                    {
                        if (udpClient13400.Available == 0)
                        {
                            await Task.Delay(100);
                            continue;
                        }

                        var receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var buffer = udpClient13400.Receive(ref receiveEndPoint);

                        if (TryGetVin(buffer, out var vin))
                        {
                            var existingDoipEntity = doipEntities.SingleOrDefault(x => x.Vin == vin);
                            if (existingDoipEntity == null)
                            {
                                var doipEntity = new DoipEntity
                                {
                                    IpAddress = receiveEndPoint.Address,
                                    Vin = vin
                                };
                                doipEntities.Add(doipEntity);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error on detecting vehicle identification message: {exception.Message}");
            }

            return doipEntities;
        }

        public static bool TryGetVin(byte[] bytes, out string vin)
        {
            var header = new Header(bytes);

            if (header.Version != DoIpCommon.DoIpVersion)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected protocol version.");
            }

            if (header.PayloadType != DoIpCommon.PayloadType.VehicleAnnouncementMessage)
            {
                vin = string.Empty;
                return false;
            }

            var vinBytes = bytes.Skip(DoIpCommon.HeaderLength).Take(DoIpCommon.VinLength).ToArray();

            if (vinBytes.All(x => x == 0) || vinBytes.All(x => x == 0xFF))
            {
                vinBytes = Enumerable.Repeat((byte)0x30, DoIpCommon.VinLength).ToArray();
            }

            vin = Encoding.ASCII.GetString(vinBytes);
            return true;
        }

        private static IPAddress? GetSingleNicAddress(NetworkInterface nic)
        {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                return GetLocalLoopbackAddress();
            }

            var nicProperties = nic.GetIPProperties();
            var iPv4 = nicProperties.UnicastAddresses.FirstOrDefault(x => x.Address.AddressFamily == AddressFamily.InterNetwork);
            return iPv4?.Address;
        }

        private static IPAddress GetLocalLoopbackAddress()
        {
            // For loopback we do not want to get in conflict with IP used by simulator(s).
            // Simulator will propose addresses 127.0.0.N, starting from N = 1.
            return new IPAddress(new byte[] { 127, 0, 0, 254 });
        }

        private static List<NetworkInterface> GetIPv4Interfaces(params NetworkInterfaceType[] wantedInterfaceTypes)
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => wantedInterfaceTypes.Contains(x.NetworkInterfaceType) && x.Supports(NetworkInterfaceComponent.IPv4))
                .ToList();
            return nics;
        }

        private static List<NetworkInterface> GetOperationalNics()
        {
            // Is there a way to know in greater detail what NICs to try? E.g. is there an IP mask we can use?
            return GetIPv4Interfaces(NetworkInterfaceType.Ethernet, NetworkInterfaceType.Loopback)
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .ToList();
        }
    }
}
