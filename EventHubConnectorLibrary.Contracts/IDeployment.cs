using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Contracts
{
    public interface IDeployment
    {
        Task<bool> Deploy(CancellationToken cancellationToken);
    }
}
