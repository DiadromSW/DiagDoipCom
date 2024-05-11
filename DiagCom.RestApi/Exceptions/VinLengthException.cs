using Utilities;

namespace DiagCom.RestApi.Exceptions
{
    /// <summary>
    /// This class represents exceptions that occur when the VIN length is not correct.
    /// </summary>
    [Serializable]
    public class VinLengthException : Exception, IExceptionCommon
    {
        /// <summary>
        /// Error code for the VIN length is not correct exception
        /// </summary>
        public ErrorCodes ErrorCode { get; } = ErrorCodes.VinLength;

        /// <summary>
        /// exceptions that occur when the VIN length is not correct
        /// </summary>
        /// <param name="message"></param>
        public VinLengthException(string message) : base(message)
        {
        }
    }
}
