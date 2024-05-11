using System;
using System.Runtime.Serialization;
using Utilities;

namespace Logging.Exceptions
{
    /// <summary>
    /// This class represents exceptions that occur when no logs exists for the user to claim.
    /// </summary>
    [Serializable]
    public class NoLogsFoundException : Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.MissingLogs;
        public NoLogsFoundException()
        {
        }

        public NoLogsFoundException(string message) : base(message)
        {
        }

        public NoLogsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoLogsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}