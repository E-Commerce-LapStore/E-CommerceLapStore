using System.ComponentModel.DataAnnotations;
using LapStore.BLL.Configuration;
using Microsoft.Extensions.Options;

namespace LapStore.BLL.Services
{
    public class ConfigurationValidationService
    {
        private readonly AppSettings _settings;

        public ConfigurationValidationService(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public void ValidateConfiguration()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(_settings);

            if (!Validator.TryValidateObject(_settings, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(r => r.ErrorMessage);
                throw new ApplicationException($"Configuration validation failed: {string.Join(", ", errors)}");
            }

            ValidateConnectionStrings();
            ValidateSecuritySettings();
        }

        private void ValidateConnectionStrings()
        {
            if (string.IsNullOrWhiteSpace(_settings.ConnectionStrings?.DefaultConnection))
            {
                throw new ApplicationException("DefaultConnection string is missing or empty");
            }
        }

        private void ValidateSecuritySettings()
        {
            var security = _settings.Security;
            if (security == null)
            {
                throw new ApplicationException("Security settings are missing");
            }

            if (security.PasswordExpirationDays <= 0)
            {
                throw new ApplicationException("PasswordExpirationDays must be greater than 0");
            }

            if (security.MaxFailedAccessAttempts <= 0)
            {
                throw new ApplicationException("MaxFailedAccessAttempts must be greater than 0");
            }

            if (security.LockoutMinutes <= 0)
            {
                throw new ApplicationException("LockoutMinutes must be greater than 0");
            }
        }
    }
} 