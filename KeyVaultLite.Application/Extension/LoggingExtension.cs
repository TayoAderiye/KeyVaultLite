using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace KeyVaultLite.Application.Extension
{
    public static class LoggingExtension
    {
        public static void AddLoggerProvider(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.WriteTo.OpenTelemetry();
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });
            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
                logging.AddConsole();
            });

            //builder.Host.UseSerilog(); 

            Log.Information("Starting up the application");

            // Default logging providers (to keep Console/Debug/EventSource too)
            builder.SetDefaultLogging();

        }
        public static void UseLoggerProvider(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "Handled {RequestPath}";
                options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

            app.Logger.LogInformation("{EnvironmentEnvironmentName} Environment ", app.Environment.EnvironmentName);
        }

        private static void SetDefaultLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Logging.AddEventSourceLogger();
        }
    }
}
