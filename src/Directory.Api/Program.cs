using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Directory.Api {
    [ExcludeFromCodeCoverage]
    public static class Program {
        private const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseIIS()
                //.ConfigureLogging(logging => logging.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace))
                .ConfigureLogging(loggingBuilder => loggingBuilder
                    .AddSerilog()
                    .AddConsole()
                    .AddDebug()
                    .SetMinimumLevel(LogLevel.Trace))
                .UseSerilog((ctx, config) => config
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                    .MinimumLevel.Override("System", LogEventLevel.Verbose)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Verbose)
                    .Enrich.FromLogContext()
                    .WriteTo.File("directory.log")
                    .AuditTo.File("audit.log")
                    .WriteTo.Console(outputTemplate: OutputTemplate, theme: AnsiConsoleTheme.Literate))
                .UseStartup<Startup>();
    }
}
