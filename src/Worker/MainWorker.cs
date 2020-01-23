// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RodriBus.UpBot.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RodriBus.UpBot.Worker
{
    /// <summary>
    /// Main worker class.
    /// </summary>
    public class MainWorker : BackgroundService
    {
        private readonly ILogger<MainWorker> Logger;
        private readonly IUpBot Bot;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public MainWorker(ILogger<MainWorker> logger, IUpBot bot)
        {
            Logger = logger;
            Bot = bot;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken).ConfigureAwait(false);
            await Bot.ReportStartupAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Bot.ExecuteBotAsync(stoppingToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Bot.ReportShutdownAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}