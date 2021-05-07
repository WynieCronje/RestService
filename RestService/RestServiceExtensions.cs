using Microsoft.Extensions.DependencyInjection;
using System;

namespace RestService
{
	public static class RestServiceExtensions
    {
        /// <summary>
        /// Adds a <see cref="IRestService"/> service. Please refer to <see cref="RestServiceConfiguration"/> for usage
        /// </summary>
        public static IServiceCollection AddRestService(this IServiceCollection services, Action<RestServiceConfiguration> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IRestService, RestService>();

            return services;
        }
    }
}
