using DiagCom.Doip.Messages;
using Utilities;
using static DiagCom.Doip.Messages.DoIpCommon;

namespace DiagCom.LocalCommunication.Messages
{
    public class RoutingActivationRequestMessage
    {
        private const int PayloadLengthWithoutOem = 7;
        private const int PayloadLengthWithOem = PayloadLengthWithoutOem + 4;

        public ushort SourceAddress { get; }
        public RoutingActivationType ActivationType { get; }
        public uint ReservedIso { get; } = 0;
        public uint? ReservedOem { get; }

        public RoutingActivationRequestMessage(ushort sourceAddress, RoutingActivationType activationType, uint? oem = null)
        {
            SourceAddress = sourceAddress;
            ActivationType = activationType;
            ReservedOem = oem;
        }

        public byte[] ToBytes()
        {
            var payloadLength = (uint)(ReservedOem.HasValue ? PayloadLengthWithOem : PayloadLengthWithoutOem);
            var header = new Header(DoIpVersion, PayloadType.RoutingActivationRequest, payloadLength);

            var bytes = header.ToBytes().ToList();
            bytes.AddRange(SourceAddress.ToBytes());
            bytes.Add((byte)ActivationType);
            bytes.AddRange(ReservedIso.ToBytes());

            if (ReservedOem.HasValue)
            {
                bytes.AddRange(ReservedOem.Value.ToBytes());
            }

            return bytes.ToArray();
        }
    }
}
