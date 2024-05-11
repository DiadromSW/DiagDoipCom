using System.Runtime.Serialization;

namespace DiagCom.Commands.Exceptions
{
    [Serializable]
    internal class SwdlOngoingException : Exception
    {
        public SwdlOngoingException()
        {
        }

        public SwdlOngoingException(string? message) : base(message)
        {
        }

        public SwdlOngoingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SwdlOngoingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}