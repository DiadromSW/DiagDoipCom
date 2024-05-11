using DiagCom.Commands.Coordination;
using DiagCom.Commands.Exceptions;
using Microsoft.Extensions.Logging;

namespace DiagCom.LocalCommunication
{
    public sealed class ConnectionController : IConnectionController
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDoipEntityMonitor _doipEntityMonitor;
        private readonly Dictionary<string, DoipEntityBroker> _vinBrokers = new();
        private readonly object _syncObject = new();

        public ConnectionController( ILoggerFactory loggerFactory, IDoipEntityMonitor doipEntityMonitor)
        {
            _logger = loggerFactory.CreateLogger<ILogger<ConnectionController>>();
            _loggerFactory = loggerFactory;
            _doipEntityMonitor = doipEntityMonitor;
        }

        public void Dispose()
        {
            lock (_syncObject)
            {
                var brokers = _vinBrokers.Keys.ToList();
                brokers.ForEach(x => Close(x));
            }
        }
       
        public List<string> GetCurrentVins()
        {
            lock (_syncObject)
            {
                _logger.LogDebug("ENTER GetVins.");
                // TODO: This need some more thought. Exactly which VINs shall be returned here?
                // Maybe all VINs that is found either on UDP or connected on TCP?
                // How should we behave during SWDL? We should probably stop the monitoring during SWDL.
                RefreshVinBrokers(_doipEntityMonitor.DoipEntities);
                var vins = _vinBrokers.Where(kvp => kvp.Value.IsPresent).Select(kvp => kvp.Key).ToList();
                _logger.LogDebug($"EXIT GetVins. Returning VINS [{string.Join(",", vins)}].");
                return vins;
            }
        }

        public void DisconnectVin(string vin)
        {
            lock (_syncObject)
            {
                GetDoipEntityBroker(vin).Disconnect();
            }
        }

        public IVinBroker GetBroker(string vin)
        {
            return GetDoipEntityBroker(vin);
        }

        private DoipEntityBroker GetDoipEntityBroker(string vin)
        {
            lock (_syncObject)
            {
                RefreshVinBrokers(_doipEntityMonitor.DoipEntities);

             
                if (_vinBrokers.ContainsKey(vin) && _vinBrokers[vin].IsPresent)
                {
                    return _vinBrokers[vin];
                }


                throw new VinNotKnownException(vin, "vin overide not added");
            }
        }

        public void RefreshVinBrokers(List<DoipEntity> foundDoipEntities)
        {
            lock (_syncObject)
            {
                // Add and/or update found DoIP entities.
                foreach (var doipEntity in foundDoipEntities)
                {
                    if (_vinBrokers.ContainsKey(doipEntity.Vin))
                    {
                        var vinBroker = _vinBrokers[doipEntity.Vin];

                        if (!vinBroker.DoipEntity.Equals(doipEntity))
                        {
                            _logger.LogDebug($"Updating existing DoIP entity (because new IP). [{doipEntity}]");
                            vinBroker.Update(doipEntity);
                        }
                    }
                    else
                    {
                        AddDoipEntity(doipEntity);
                    }

                    _vinBrokers[doipEntity.Vin].IsFoundOnUdp = true;
                }

                // Update DoIP entities not found.
                foreach (var vinBroker in _vinBrokers)
                {
                    if (!foundDoipEntities.Any(x => x.Vin == vinBroker.Key))
                    {
                        if (vinBroker.Value.IsFoundOnUdp)
                        {
                            _logger.LogDebug($"Updating existing DoIP entity to not found on DoIP. {vinBroker.Value.DoipEntity}.");
                            vinBroker.Value.IsFoundOnUdp = false;
                        }
                    }
                }

                // TODO M2:
                // Add a strategy to close/remove brokers.
                // A broker can be closed when it has no state information that we must keep.
                // Closing should be synchronized with other activities so we do not close when an SWDL is starting.
                // Also consider if we need to close state-holding brokers after some time of inactivity.
            }
        }

        private void AddDoipEntity(DoipEntity doipEntity)
        {
            lock (_syncObject)
            {
                _logger.LogDebug($"Adding new DoIP entity. [{doipEntity}]");
                var vinBroker = new DoipEntityBroker(doipEntity, _loggerFactory);
                _vinBrokers.Add(doipEntity.Vin, vinBroker);
            }
        }

        private void Close(string vin)
        {
            lock (_syncObject)
            {
                var vinBroker = _vinBrokers[vin];
                vinBroker.Dispose();
                _vinBrokers.Remove(vin);
            }
        }
    }
}
