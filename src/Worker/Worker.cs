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
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> Logger;
        private readonly IUpBot Bot;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public Worker(ILogger<Worker> logger, IUpBot bot)
        {
            Logger = logger;
            Bot = bot;
        }

        /// <summary>
        /// This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Bot.LogSelfAsync(stoppingToken).ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken).ConfigureAwait(false);
            }
        }
    }
}