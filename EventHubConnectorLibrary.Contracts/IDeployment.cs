using System.Threading;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Contracts
{
    public interface IDeployment
    {
        Task<bool> Deploy(CancellationToken cancellationToken, string[] args = null);
    }
}