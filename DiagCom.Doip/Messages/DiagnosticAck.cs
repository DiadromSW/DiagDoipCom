using DiagCom.Doip.Exceptions;
using Utilities;

namespace DiagCom.Doip.Messages
{

    public class DiagnosticAck : IMessage
    {
        private const int PayloadLengthWithoutData = 5;

        public ushort SourceAddress { get; private set; }
        public ushort TargetAddress { get; private set; }
        public byte AckCode { get; private set; }
        public byte[] Data { get; set; }

        public DiagnosticAck(byte[] bytes)
        {
            if (bytes.Length < DoIpCommon.HeaderLength + PayloadLengthWithoutData)
            {
                throw new CommunicationException("Received message to short.");
            }

            Header header = new Header(bytes);

            if (header.Version != DoIpCommon.DoIpVersion)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected protocol version.");
            }

            if (header.PayloadType != DoIpCommon.PayloadType.DiagnosticAck)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected payload type.");
            }

            int pos = DoIpCommon.HeaderLength;

            SourceAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;
            TargetAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;

            AckCode = bytes[pos++];

            Data = new byte[header.PayloadLength - PayloadLengthWithoutData];
            Array.Copy(bytes, pos, Data, 0, Data.Length);

            Length = pos + Data.Length;
        }

        public int Length
        {
            get;
            private set;
        }
    }
}
