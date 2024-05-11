using System.Runtime.Serialization;
using Utilities;

namespace DiagCom.Doip.Exceptions
{
    /// <summary>
    /// This class handles the potential exceptions that arises from communication between
    /// program and car.
    /// </summary>
    [Serializable]
    public class CommunicationException : Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.VehicleCommunication;
        public CommunicationException()
        {
        }

        public CommunicationException(string message) : base(message)
        {
        }

        public CommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}