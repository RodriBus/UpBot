// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using System.Threading;
using System.Threading.Tasks;

namespace RodriBus.UpBot.Application.Services
{
    /// <summary>
    /// Represents contracts with UpBot functionality.
    /// </summary>
    public interface IUpBot
    {
        /// <summary>
        /// Prints self information into log.
        /// </summary>
        Task LogSelfAsync(CancellationToken stoppingToken);
    }
}