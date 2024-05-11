namespace DiagCom.LocalCommunication
{
    public sealed class DoipEntityMonitor : IDoipEntityMonitor
    {
        private const int TimeBetweenLookupsMs = 1000;
        private readonly EthernetDoipEntityLookup _doipEntityLookup;
        private Thread? _thread;
        private volatile bool _stop;

        public DoipEntityMonitor(EthernetDoipEntityLookup doipEntityLookup)
        {
            _doipEntityLookup = doipEntityLookup;
        }

        private async void RunMonitoring()
        {
            while (!_stop)
            {
                DoipEntities = await _doipEntityLookup.FindAll();
                await Task.Delay(TimeBetweenLookupsMs);
            }
        }

        public List<DoipEntity> DoipEntities { get; private set; } = new();

        public async Task StartMonitoringAsync(CancellationToken stoppingToken)
        {
            await CheckForDoipEntitiesAsync(stoppingToken);
        }

        public void Start()
        {
            _thread = new Thread(RunMonitoring);
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread == null)
            {
                return;
            }

            _stop = true;
            _thread.Join();
            _thread = null;
        }

        private async Task CheckForDoipEntitiesAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DoipEntities = await _doipEntityLookup.FindAll();
                await Task.Delay(TimeBetweenLookupsMs, stoppingToken);
            }
        }

        public void EnsureDoipEntity(DoipEntity doipEntity)
        {
            DoipEntities.Add(doipEntity);
        }
    }
}
