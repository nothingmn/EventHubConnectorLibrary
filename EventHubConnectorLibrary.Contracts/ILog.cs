using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Contracts
{
    public interface ILog
    {
        Task Debug(string message, params object[] args);

        Task Info(string message, params object[] args);

        Task Event(string eventName, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);

        Task Error(System.Exception exception, string message, params object[] args);

        Task Fatal(System.Exception exception, string message, params object[] args);
    }
}