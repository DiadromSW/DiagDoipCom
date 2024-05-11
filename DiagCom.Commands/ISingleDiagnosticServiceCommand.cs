using DiagCom.Commands.Coordination;

namespace DiagCom.Commands
{
    public class ServiceResponse
    {
        public ServiceResponse()
        {
        }

        public ServiceResponse(string hex)
            : this(ResponseType.RESPONSE, Convert.ToUInt16(hex[0..4], 16), Convert.FromHexString(hex.Skip(8).ToArray()))
        {
        }

        private ServiceResponse(ResponseType type, ushort sourceAddress, byte[] response)
        {
            ResponseType = type;
            SourceAddress = sourceAddress;
            Response = response;
        }

        public ushort SourceAddress { get; set; }
        public byte[] Response { get; set; } = new byte[0];

        public ResponseType ResponseType { get; set; }
        public override string ToString()
        {
            return $"{SourceAddress.ToString("X4")} {Convert.ToHexString(Response)}";
        }
    }

    public interface ISingleDiagnosticServiceCommand
    {
        Task<List<string>> ExecuteAsync(IVinBroker vinBroker, IProgress<ServiceResponse>? progress = null);
        Task<List<string>> ExecuteAsync(IExecutionContext executionContext, IProgress<ServiceResponse>? progress = null);
    }
}

namespace DiagCom.Commands
{
    public enum ResponseType
    {
        ACK,
        NACK,
        RESPONSE
    }
}