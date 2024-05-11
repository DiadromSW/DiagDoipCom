using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.Threading;

namespace Utilities.Test
{
    public class DoipTcpTestServer
    {
        private TcpListener _tcpListener;
        private AsyncManualResetEvent _stopListeningEvent = new AsyncManualResetEvent(false);
        private Task? _listenerTask;
        public Func<byte[], byte[]>? HandleRequest { get; set; } = null;

        public DoipTcpTestServer(string ipAddress = "127.0.0.1")
            : this(IPAddress.Parse(ipAddress))
        {
        }

        public DoipTcpTestServer(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
            _tcpListener = new TcpListener(IpAddress, 13400);
        }

        public IPAddress IpAddress { get; }

        public void Start()
        {
            _tcpListener.Start();
            _listenerTask = RunListenerAsync();
        }

        public void Stop()
        {
            _stopListeningEvent.Set();

            if (_listenerTask != null)
            {
                _listenerTask.Wait();
            }
        }

        private async Task RunListenerAsync()
        {
            using var client = await _tcpListener.AcceptTcpClientAsync();
            var stream = client.GetStream();
            var stopTask = _stopListeningEvent.WaitAsync();
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    var readTask = stream.ReadAsync(buffer, 0, buffer.Length);
                    if (await Task.WhenAny(readTask, stopTask) == stopTask)
                    {
                        break;
                    }

                    var noOfBytesRead = await readTask;
                    if (noOfBytesRead > 0)
                    {
                        var response = HandleRequest?.Invoke(buffer.Take(noOfBytesRead).ToArray());

                        if (response?.Length > 0)
                        {
                            var writeTask = stream.WriteAsync(response).AsTask();
                            if (await Task.WhenAny(writeTask, stopTask) == stopTask)
                            {
                                break;
                            }
                        }

                        continue;
                    }

                    break;
                }
                catch (Exception)
                {
                    break;
                }
            }

            try
            {
                _tcpListener.Stop();
            }
            catch (Exception)
            {
                // Ignore.
            }
        }
    }
}
