using Microsoft.Extensions.Logging;
using System.Diagnostics;
using LapStore.BLL.Interfaces;

namespace LapStore.BLL.Services
{

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogError(string message, Exception ex, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogPerformance(string operationName, long elapsedMilliseconds)
        {
            if (elapsedMilliseconds > 1000) // Log if operation takes more than 1 second
            {
                _logger.LogWarning("Performance warning: {Operation} took {ElapsedMs}ms", 
                    operationName, elapsedMilliseconds);
            }
            else
            {
                _logger.LogDebug("Operation {Operation} completed in {ElapsedMs}ms", 
                    operationName, elapsedMilliseconds);
            }
        }
    }
} 