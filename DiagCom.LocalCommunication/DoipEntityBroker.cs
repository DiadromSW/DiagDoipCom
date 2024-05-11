using DiagCom.Commands.Coordination;
using DiagCom.Doip;
using DiagCom.Uds;
using Microsoft.Extensions.Logging;

namespace DiagCom.LocalCommunication
{
    public sealed class DoipEntityBroker : IDoipEntityBroker
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly EthernetConnectionManager _connectionManager;
        private readonly LocalCommunicator _communicator;
        private readonly MessageHandler _messageHandler;

        public DoipEntityBroker(DoipEntity doipEntity, ILoggerFactory loggerFactory)
        {
            DoipEntity = doipEntity;
            _loggerFactory = loggerFactory;

            var iso14229Logger = new Iso14229Logger(CreateLogger("ISO 14229"));
            _connectionManager = new EthernetConnectionManager(doipEntity, CreateLogger<EthernetConnectionManager>(), iso14229Logger);
            var messageParser = new MessageParser(CreateLogger<MessageParser>());
            _communicator = new LocalCommunicator(_connectionManager, messageParser);
            _messageHandler = new MessageHandler(_communicator, CreateLogger<MessageHandler>());
            OperationController = new OperationsController(_messageHandler, CreateLogger<OperationsController>());
            TesterPresentController = new TesterPresentController(_messageHandler);
       
        }

        public void Dispose()
        {
            TesterPresentController.Dispose();
            _messageHandler.Dispose();
            _connectionManager.Dispose();
            _communicator.Dispose();
        }

        public DoipEntity DoipEntity { get; private set; }

        public ITesterPresentController TesterPresentController { get; }
        public IOperationController OperationController { get; }

        public bool IsPresent => IsFoundOnUdp || IsTcpConnected;

        public bool IsFoundOnUdp { get; set; }

        public bool IsTcpConnected => _connectionManager.IsConnected();

        public string Vin => DoipEntity.Vin;

        public void Update(DoipEntity doipEntity)
        {
            if (doipEntity.Vin != DoipEntity.Vin)
            {
                throw new Exception("Should NOT occur!");
            }

            DoipEntity = doipEntity;
        }

        internal void Disconnect()
        {
            _connectionManager.Disconnect();
        }

        public ILogger CreateLogger(string name)
        {
            var loggerName = $"Vin.{name}.{DoipEntity.Vin}";
            return _loggerFactory.CreateLogger(loggerName);
        }

        private ILogger CreateLogger<T>()
        {
            return CreateLogger(typeof(T).Name);
        }
    }
}