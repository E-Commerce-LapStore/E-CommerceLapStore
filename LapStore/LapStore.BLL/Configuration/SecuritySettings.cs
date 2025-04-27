using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Configuration
{
    public class SecuritySettings
    {
        public int PasswordExpirationDays { get; set; } = 90;
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 30;
        public bool RequireHttps { get; set; } = true;
        public string[] CorsAllowedOrigins { get; set; }
    }
}
