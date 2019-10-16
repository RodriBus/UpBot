// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodriBus.UpBot.Application.Extensions;
using Serilog;

namespace RodriBus.UpBot.Worker
{
    /// <summary>
    /// Application main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates and configure the host builder.
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddApplicationServices(hostContext.Configuration);

                services.AddHostedService<MainWorker>();
            })
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
            , writeToProviders: true)
            ;
    }
}