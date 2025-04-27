using LapStore.BLL.Configuration;
using LapStore.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LapStore.BLL.DependencyInjections
{
    public static class ConfigurationDependencyInjection
    {
        public static IServiceCollection AddConfigurationDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind and validate AppSettings
            services.Configure<AppSettings>(configuration);
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Add configuration validation service
            services.AddSingleton<ConfigurationValidationService>();

            // Validate configuration on startup
            var sp = services.BuildServiceProvider();
            var validationService = sp.GetRequiredService<ConfigurationValidationService>();
            validationService.ValidateConfiguration();

            return services;
        }
    }
} 