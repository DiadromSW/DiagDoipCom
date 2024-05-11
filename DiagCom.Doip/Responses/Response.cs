using DiagCom.Doip.Messages;
using DiagCom.Doip.Responses.EcuResponses;

namespace DiagCom.Doip.Responses
{
    public class Response : IResponse
    {
        public byte[] Data { get; set; }

        public IMessage Message { get; set; }

        internal Response(byte[] data)
        {
            Data = data;

        }

        internal Response(IMessage x)
        {
            Message = x;
        }
    }

    public class PositiveResponse : Response
    {
        public PositiveResponse(IMessage message) : base(message)
        {
        }
    }

    public class NegativeResponse : Response
    {
        public EcuResponseCodes EcuNegativeResponse { get; set; }

        public NegativeResponse(byte[] data) : base(data)
        {
        }
        public NegativeResponse(IMessage message) : base(message)
        {
            //First, second and third bytes always comes as:
            //[NegativeResponse][Identifier][EcuNegativeResponseCode]
            if (message.Data.Count() >= 2)
                EcuNegativeResponse = (EcuResponseCodes)message.Data[2];
            else
                EcuNegativeResponse = EcuResponseCodes.Undefined;
        }
    }
    public class PendingResponse : Response
    {

        public PendingResponse(byte[] data) : base(data)
        {
        }
        public PendingResponse(IMessage message) : base(message)
        {
        }
    }

}
