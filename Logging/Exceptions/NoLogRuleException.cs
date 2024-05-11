using System.Runtime.Serialization;
using Utilities;

namespace Logging.Exceptions
{
    /// <summary>
    /// This class represents exceptions when there is no corresponding rule on how log in the logging rule list.
    /// </summary>
    [Serializable]
    public class NoLogRuleException : Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.MissingLogRule;
        public NoLogRuleException()
        {
        }

        public NoLogRuleException(string message) : base(message)
        {
        }

        public NoLogRuleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoLogRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}