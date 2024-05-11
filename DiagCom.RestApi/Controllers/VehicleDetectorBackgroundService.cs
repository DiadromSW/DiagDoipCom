using DiagCom.LocalCommunication;
using Microsoft.Extensions.Hosting;

namespace DiagCom.RestApi.Controllers
{
    /// <summary>
    /// Background Service that is responsable for vehicle identification
    /// </summary>
    public sealed class VehicleDetectorBackgroundService : BackgroundService
    {
        private readonly IDoipEntityMonitor _doipEntityMonitor;

        /// <summary>
        /// Background Service that is responsable for vehicle identification
        /// </summary>
        /// <param name="doipEntityMonitor"></param>
        public VehicleDetectorBackgroundService(IDoipEntityMonitor doipEntityMonitor)
        {
            _doipEntityMonitor = doipEntityMonitor;
        }

        /// <summary>
        /// Start monitoring vehicles on network
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _doipEntityMonitor.StartMonitoringAsync(stoppingToken);
        }
    }
}