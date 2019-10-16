// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using RodriBus.UpBot.Application.Configuration;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RodriBus.UpBot.Application.Services
{
    /// <summary>
    /// Configuration manager that manages settings through file system.
    /// </summary>
    public class FileConfigurationManager : IConfigurationManager
    {
        /// <summary>
        /// Gets an instance of current settings.
        /// </summary>
        /// <returns>An instance of configuration values.</returns>
        public async Task<UpBotSettings> GetConfigutationAsync(CancellationToken cancellationToken = default)
        {
            await EnsureConfigFileAsync(cancellationToken).ConfigureAwait(false);
            var path = GetConfigFilePath();
            var fileContent = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            return JsonSerializer.Deserialize<UpBotSettings>(fileContent);
        }

        /// <summary>
        /// Saves settings into persistence system.
        /// </summary>
        /// <param name="instance">Instance containing values to be saved.</param>
        /// <param name="cancellationToken">Indicates that the operation should be cancelled.</param>
        public async Task SetConfigutationAsync(UpBotSettings instance, CancellationToken cancellationToken = default)
        {
            var text = JsonSerializer.Serialize(instance, new JsonSerializerOptions { WriteIndented = true });
            await EnsureConfigFileAsync(cancellationToken).ConfigureAwait(false);
            var configPath = GetConfigFilePath();

            await File.WriteAllTextAsync(configPath, text, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
        }

        private string GetConfigFolderPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            return Path.Combine(appData, "RodriBus", "UpBot");
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(GetConfigFolderPath(), "upbot.config.json");
        }

        private async Task EnsureConfigFileAsync(CancellationToken cancellationToken = default)
        {
            var directory = GetConfigFolderPath();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var config = GetConfigFilePath();
            if (!File.Exists(config))
            {
                using var file = File.CreateText(config);
                await file.WriteLineAsync("{}".AsMemory(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}