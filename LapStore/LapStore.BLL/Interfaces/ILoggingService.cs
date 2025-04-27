using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.Interfaces
{
    public interface ILoggingService
    {
        void LogError(string message, Exception ex, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogPerformance(string operationName, long elapsedMilliseconds);
    }
}
