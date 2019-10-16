// Copyright (c) 2019 Diego Rodríguez Suárez-Bustillo <diego@rodribus.com>. Licensed under the MIT Licence.
// See the LICENSE file in the repository root for full licence text.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RodriBus.UpBot.Application.Configuration;
using RodriBus.UpBot.Application.Services;

namespace RodriBus.UpBot.Application.Extensions
{
    /// <summary>
    /// Contais extension methods to configure application services into IoC.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds application services.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UpBotConfiguration>(configuration);

            services.AddHttpClient<IUpBot, Services.UpBot>();

            services.AddTransient<IConfigurationManager, FileConfigurationManager>();

            return services;
        }
    }
}