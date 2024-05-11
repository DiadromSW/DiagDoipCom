using Utilities;

namespace DiagCom.Commands.Exceptions
{
    [Serializable]
    public class TimeoutWaitingForSwdlExclusivityException : Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.SwdlExclusivity;
        public TimeoutWaitingForSwdlExclusivityException() : base("SoftwareDownload could not start because of other operation running")
        {
        }
    }
}