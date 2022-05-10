//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Boilerplate.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Topshelf;
using Host = Microsoft.Extensions.Hosting.Host;

namespace Boilerplate.Service
{
    /// <summary>
    /// Boilerplate Service Program class
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<IHost>(s =>
                {
                    s.ConstructUsing(() => CreateHostBuilder(args).Build());
                    s.WhenStarted(service =>
                    {
                        service.Start();
                    });

                    s.WhenStopped(service =>
                    {
                        service.StopAsync();
                    });
                });

                x.StartAutomatically();
                x.RunAsLocalSystem();

                x.SetServiceName("Boilerplate.Service.ServiceName");
                x.SetDisplayName("Boilerplate.Service.DisplayName");
                x.SetDescription("Boilerplate.Service.Description");
            });
        }

        /// <summary>
        /// Create HostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if DEBUG
                 .UseEnvironment(Environments.Development)
#else
                .UseEnvironment(Environments.Production)
                //.UseEnvironment(Environments.Staging)
#endif
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("appsettings.json", optional: true);
#if DEBUG
                    configHost.AddJsonFile("appsettings.Development.json", optional: true);
#endif
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                //.UseSerilog()
                .ConfigureLogging((context, logger) =>
                {
                    logger.AddConsole();
                    //logger.AddLog4Net();
                    logger.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

