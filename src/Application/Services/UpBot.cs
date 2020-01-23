// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RodriBus.UpBot.Application.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RodriBus.UpBot.Application.Services
{
    /// <summary>
    /// Implements UpBot contracts.
    /// </summary>
    public class UpBot : IUpBot
    {
        private const string EMOJI_ON = "☀";
        private const string EMOJI_OFF = "🌙";

        private ILogger<UpBot> Logger { get; }
        private HttpClient HttpClient { get; }
        private UpBotConfiguration Configuration { get; }
        private TelegramBotClient Bot { get; }
        private IConfigurationManager Config { get; }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public UpBot(ILogger<UpBot> logger, IOptions<UpBotConfiguration> configuration, HttpClient httpClient, IConfigurationManager config)
        {
            Logger = logger;
            Configuration = configuration.Value;
            HttpClient = httpClient;
            Config = config;
            Bot = new TelegramBotClient(Configuration.AccessToken, HttpClient);
        }

        /// <summary>
        /// Starts bot listening tasks.
        /// </summary>
        public async Task ExecuteBotAsync(CancellationToken cancellationToken = default)
        {
            Bot.OnMessage += Bot_OnMessageAsync;
            Bot.StartReceiving(null, cancellationToken);

            var me = await Bot.GetMeAsync(cancellationToken).ConfigureAwait(false);

            Logger.LogDebug("Bot initialized as '({Id})@{UserName}: {FirstName}'.", me.Id, me.Username, me.FirstName);
        }

        /// <summary>
        /// Reports startup to configured chat.
        /// </summary>
        public async Task ReportStartupAsync(CancellationToken cancellationToken = default)
        {
            var config = await Config.GetConfigutationAsync(cancellationToken).ConfigureAwait(false);
            if (config.ChatId != 0)
            {
                _ = await Bot.SendTextMessageAsync(
                    chatId: config.ChatId,
                    text: $"{EMOJI_ON} Hey! I'm starting up."
                    ).ConfigureAwait(false);
            }
            else
            {
                Logger.LogWarning("UpBot chat to report was not found.");
            }
        }

        /// <summary>
        /// Reports shutting down to configured chat.
        /// </summary>
        public async Task ReportShutdownAsync(CancellationToken cancellationToken = default)
        {
            var config = await Config.GetConfigutationAsync(cancellationToken).ConfigureAwait(false);
            await Bot.SendTextMessageAsync(
                chatId: config.ChatId,
                text: $"{EMOJI_OFF} Oh! I'm shutting down."
                ).ConfigureAwait(false);
        }

        private async void Bot_OnMessageAsync(object sender, MessageEventArgs e)
        {
            //TODO: Improve this to be more generic
            if (e.Message.Text != null)
            {
                Logger.LogDebug($"Received a text message in chat {e.Message.Chat.Id}: {e.Message.Text}");
            }
            if (e.Message.Text?.StartsWith("/reporthere") == true)
            {
                await SaveChatIdAsync(e).ConfigureAwait(false);
            }
            if (e.Message.Text?.StartsWith("/showsettings") == true)
            {
                await GetSettingsAsStringAsync(e).ConfigureAwait(false);
            }
        }

        private async Task SaveChatIdAsync(MessageEventArgs e)
        {
            var chatId = e.Message.Chat.Id;
            var settings = await Config.GetConfigutationAsync().ConfigureAwait(false);
            settings.ChatId = chatId;
            await Config.SetConfigutationAsync(settings).ConfigureAwait(false);
            await Bot.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: $"From now on, I will report you to this chat. (ChatId: {chatId})"
                ).ConfigureAwait(false);
        }

        private async Task GetSettingsAsStringAsync(MessageEventArgs e)
        {
            var settings = await Config.GetConfigutationAsync().ConfigureAwait(false);
            var text = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await Bot.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: text
                ).ConfigureAwait(false);
        }
    }
}