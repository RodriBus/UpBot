// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using RodriBus.UpBot.Application.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace RodriBus.UpBot.Application.Services
{
    /// <summary>
    /// Upbot configuration manager contract.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets an instance of current settings.
        /// </summary>
        /// <returns>An instance of configuration values.</returns>
        /// <param name="cancellationToken">Indicates that the operation should be cancelled.</param>
        Task<UpBotSettings> GetConfigutationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves settings into persistence system.
        /// </summary>
        /// <param name="instance">Instance containing values to be saved.</param>
        /// <param name="cancellationToken">Indicates that the operation should be cancelled.</param>
        Task SetConfigutationAsync(UpBotSettings instance, CancellationToken cancellationToken = default);
    }
}