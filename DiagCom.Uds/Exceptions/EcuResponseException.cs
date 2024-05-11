using DiagCom.Doip.Responses.EcuResponses;
using System.Runtime.Serialization;
using Utilities;

namespace DiagCom.Uds.Exceptions
{
    [Serializable]
    public class EcuResponseException : Exception, IExceptionCommon
    {
        public ErrorCodes ErrorCode { get; } = ErrorCodes.EcuNegativeResponse;

        public EcuResponseException(EcuResponseCodes code, string msg) : base($"{msg}: {Enum.GetName(typeof(EcuResponseCodes), code)}")
        {

        }

        public EcuResponseException(string message) : base(message)
        {
        }
    }
}
