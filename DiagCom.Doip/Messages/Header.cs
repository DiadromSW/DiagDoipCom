using DiagCom.Doip.Exceptions;

namespace DiagCom.Doip.Messages
{
    public class Header
    {
        public byte Version { get; private set; }
        public DoIpCommon.PayloadType PayloadType { get; private set; }
        public uint PayloadLength { get; private set; }

        public Header(byte version, DoIpCommon.PayloadType payloadType, uint payloadLength)
        {
            Version = version;
            PayloadType = payloadType;
            PayloadLength = payloadLength;
        }

        public Header(byte[] bytes)
        {
            if (bytes.Length < DoIpCommon.HeaderLength)
            {
                throw new CommunicationException("Received header to short");
            }

            Version = bytes[0];
            PayloadType = (DoIpCommon.PayloadType)((bytes[2] << 8) + bytes[3]);
            PayloadLength = (uint)((bytes[4] << 24) + (bytes[5] << 16) + (bytes[6] << 8) + bytes[7]);
        }
        public byte[] ToBytes()
        {
            return new[]
            {
                Version, (byte)(~Version),
                (byte)((ushort)PayloadType >> 8), (byte)((ushort)PayloadType & 0xFF),
                (byte)(PayloadLength >> 24), (byte)(PayloadLength >> 16), (byte)(PayloadLength >> 8), (byte)(PayloadLength & 0xFF)
            };
        }
    }
}
