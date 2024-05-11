using DiagCom.RestApi.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;
using Utilities;

namespace RestApi
{
    public static class Program
    {
        private static HostConfig _hostConfig;
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                SystemInfo.SaveDiagComVersion(host.Services.GetRequiredService<IOptions<DiagComVersion>>().Value.Version);
                host.Run();
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
            finally
            {
                // Flush and close down internal threads and timers
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureServices((context, services) =>
             {
                 services.AddTransient<IHostConfig, HostConfig>();
                 _hostConfig = (HostConfig)services.BuildServiceProvider().GetRequiredService<IHostConfig>();
                 _hostConfig.Url = context.Configuration["HostConfig:Endpoints:Https:Url"];

             })
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(opt =>
                    {

                        opt.ListenAnyIP(_hostConfig.GetPort(), listOpt =>
                        {
                            //use configuration path and password
                            listOpt.UseHttps(_hostConfig.GetFullCertPath(), _hostConfig.Password);
                        });
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.ClearProviders();
                })
                .UseNLog()  // NLog: Setup NLog for Dependency injection;

                .ConfigureWebHost(config =>
                {
                    config.UseUrls("https://localhost:5001");
                })
                .UseWindowsService();
    }

}
