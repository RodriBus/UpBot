// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RodriBus.UpBot.Application.Extensions;
using Serilog;
using System;
using System.Runtime.InteropServices;

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
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Create Host
            var host = Host.CreateDefaultBuilder(args);

            // Determine host lifetime based on OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                host.UseWindowsService();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                host.UseSystemd();
            }
            else
            {
                throw new NotSupportedException("Operative system not suported: " +
                   $"${RuntimeInformation.OSDescription} ${RuntimeInformation.OSArchitecture}");
            }

            // Configure container services
            host.ConfigureServices((hostContext, services) =>
             {
                 services.AddApplicationServices(hostContext.Configuration);

                 services.AddHostedService<MainWorker>();
             });

            // Set serilog as logging provider
            host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
            , writeToProviders: true);

            return host;
        }
    }
}