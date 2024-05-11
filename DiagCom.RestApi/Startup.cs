using DiagCom.Commands.Coordination;
using DiagCom.LocalCommunication;
using DiagCom.RestApi;
using DiagCom.RestApi.Controllers;
using DiagCom.RestApi.Options;
using DiagCom.RestApi.Validators;
using DiagCom.RestApi.Validators.Configuration;
using FluentValidation.AspNetCore;
using Logging;
using Logging.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Utilities;

namespace RestApi
{
    public class Startup
    {
        private readonly string _diagComPolicy = "DiagComPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Used in testing to turn off vehicle detection where not needed in order to enhance performance.
        public static bool EnableVehicleDetection { get; set; } = true;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Decrypt values within Configuration
 
            services.AddControllers();
            services.AddMvc
          (
              options => options.Filters.Add<ValidationFilter>()
          )
          .AddFluentValidation(fv =>
          {
          });
            services.AddPayloadValidators();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DiagCom API",
                    Version = "v1",
                    // Todo: extend/edit description of API
                    Description = "A ASP.NET Core Web API"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
 
            services.AddCors(options =>
            {
                options.AddPolicy(name: _diagComPolicy,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            services.AddHttpClient();

            services.AddOptions();
            services.Configure<DiagComVersion>(value => Configuration.Bind(value));
            services.Configure<LogSettings>(value => Configuration.GetSection(LogSettings.Name).Bind(value));

            services.AddTransient<ILocalParser, LocalParser>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IDiagnosticSequenceParser, DiagnosticSequenceParser>();
            services.AddTransient<ILoggerFactory, LoggerFactory>();

            services.AddTransient<ILogHandler, LogHandler>();
            services.AddTransient<IFileWrapper, FileWrapper>();
            services.AddSingleton<EthernetDoipEntityLookup>();
            services.AddSingleton<IDoipEntityMonitor, DoipEntityMonitor>();
            services.AddSingleton<IConnectionController, ConnectionController>();
            services.AddTransient<IDiagComApi, DiagComApi>();
            services.AddMemoryCache();
            services.AddSignalR();

            if (EnableVehicleDetection)
            {
                services.AddHostedService<VehicleDetectorBackgroundService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/error");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiagCom API v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseWebSockets();
            app.UseRouting();
            app.UseCors(_diagComPolicy);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}