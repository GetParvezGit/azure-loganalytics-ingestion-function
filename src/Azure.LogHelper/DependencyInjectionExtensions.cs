using AzureLogHelper.Contracts;
using AzureLogHelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AzureLogHelper
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AzureLoggerService(this IServiceCollection services)
        {

            services.AddScoped<ILogIngestionHelper, LogIngestionHelper>();

            return services;
        }
    }
}
