namespace DiagCom.LocalCommunication
{
    public interface IDoipEntityMonitor
    {
        Task StartMonitoringAsync(CancellationToken stoppingToken);
        List<DoipEntity> DoipEntities { get; }
    }
}