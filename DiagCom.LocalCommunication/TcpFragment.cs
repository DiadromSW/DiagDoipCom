using DiagCom.Doip.Connections;

namespace DiagCom.LocalCommunication
{
    public class TcpFragment
    {
        private byte[] _data = Array.Empty<byte>();

        public byte[] Data
        {
            get => _data;
            set => _data = value;
        }

        public void Clear()
        {
            _data = Array.Empty<byte>();
        }
    }
}
