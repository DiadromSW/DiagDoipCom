using System.Runtime.Serialization;
using Utilities;

namespace Logging.Exceptions
{
    [Serializable]
    public class LogFileOpenException: Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.Logging;
     
        public LogFileOpenException(string message, Exception innerException) : base(message, innerException)
        {
        }
   
    }
}
