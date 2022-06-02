using Boilerplate.Web.Context;
using Boilerplate.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Boilerplate.Web.Controllers;

namespace Boilerplate.Web
{
    /// <summary>
    /// Boilerplate Web Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program main constructor
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExistsAsync(host).Wait();
            host.Run();
        }

        /// <summary>
        /// Create Db If Not Exists
        /// </summary>
        /// <param name="host"></param>
        public static async Task CreateDbIfNotExistsAsync(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BoilerplateContext>();
                    await DbInitializer.InitializeAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

            await Task.Delay(0);
        }

        /// <summary>
        /// Create HostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
//#if DEBUG
//                 .UseEnvironment(Environments.Development)
//#else
//                .UseEnvironment(Environments.Production)
//                //.UseEnvironment(Environments.Staging)
//#endif
//                .ConfigureHostConfiguration(configHost =>
//                {
//                    configHost.SetBasePath(Directory.GetCurrentDirectory());
//                    configHost.AddJsonFile("appsettings.json", optional: true);
//#if DEBUG
//                    configHost.AddJsonFile("appsettings.Development.json", optional: true);
//#endif
//                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
//                    configHost.AddCommandLine(args);
//                })
//                //.UseSerilog()
//                .ConfigureLogging((context, logger) =>
//                {
//                    logger.AddConsole();
//                    //logger.AddLog4Net();
//                    logger.SetMinimumLevel(LogLevel.Information);
//                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
