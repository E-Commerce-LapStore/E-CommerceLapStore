using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public LoggingSettings Logging { get; set; }
        public string AllowedHosts { get; set; }
        public SecuritySettings Security { get; set; }
    }
} 