using Microsoft.Extensions.Logging;

namespace DiagCom.Commands.Coordination
{
    public class ExecutionContext : IExecutionContext
    {
        private readonly IConnectionController _connectionController;
        private readonly ILoggerFactory _loggerFactory;
         private ILocalParser? _localParser;

        public ExecutionContext(string vin, IConnectionController connectionController, ILoggerFactory loggerFactory)
        {
            Vin = vin;
            _connectionController = connectionController;
            _loggerFactory = loggerFactory;
            ApiLogger = CreateVinLogger("API");
        }

        public string Vin { get; }
        public ILogger ApiLogger { get; }
        public IVinBroker VinBroker => _connectionController.GetBroker(Vin);

        public ILogger CreateVinLogger(string category)
        {
            var loggerName = $"Vin.{category}.{Vin}";
            return _loggerFactory.CreateLogger(loggerName);
        }

     
        public ILocalParser LocalParser
        {
            get
            {
                if (_localParser == null)
                {
                    throw new InvalidOperationException("No ILocalParser available.");
                }

                return _localParser;
            }

            init => _localParser = value;
        }

        public List<string> GetCurrentVins() => _connectionController.GetCurrentVins();
    }
}
