using System.Runtime.Serialization;

namespace DiagCom.Commands.Exceptions
{
    [Serializable]
    public class UnconfirmedSwdlException : Exception
    {
        public UnconfirmedSwdlException() : base("Another software download is ongoing. Please run software confirm before attempting again")
        {
        }

        public UnconfirmedSwdlException(string message) : base(message)
        {
        }

        public UnconfirmedSwdlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnconfirmedSwdlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}