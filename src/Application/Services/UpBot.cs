// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RodriBus.UpBot.Application.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace RodriBus.UpBot.Application.Services
{
    /// <summary>
    /// Implements UpBot contracts.
    /// </summary>
    public class UpBot : IUpBot
    {
        private ILogger<UpBot> Logger { get; }
        private HttpClient HttpClient { get; }
        private UpBotConfiguration Configuration { get; }
        private TelegramBotClient Bot { get; }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public UpBot(ILogger<UpBot> logger, IOptions<UpBotConfiguration> configuration, HttpClient httpClient)
        {
            Logger = logger;
            Configuration = configuration.Value;
            HttpClient = httpClient;
            Bot = new TelegramBotClient(Configuration.AccessToken, HttpClient);
        }

        /// <summary>
        /// Prints self information into log.
        /// </summary>
        public async Task LogSelfAsync(CancellationToken stoppingToken = default)
        {
            var me = await Bot.GetMeAsync(stoppingToken).ConfigureAwait(false);

            Logger.LogDebug("Bot initialized as '({Id})@{UserName}: {FirstName}'.", me.Id, me.Username, me.FirstName);
        }
    }
}