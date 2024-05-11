using DiagCom.Doip.Messages;

namespace DiagCom.LocalCommunication.Messages
{
    public class ResponseMessage : IMessage
    {
        public ResponseMessage(byte[] bytes)
        {
            Header header = new Header(bytes);
            Length = DoIpCommon.HeaderLength + (int)header.PayloadLength;
        }

        public int Length
        {
            get;
            private set;
        }

        public ushort SourceAddress { get; set; }

        public ushort TargetAddress { get; set; }

        public byte[] Data { get; set; }
    }
}
