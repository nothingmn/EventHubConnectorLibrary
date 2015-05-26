using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventHubConnectorLibrary.Contracts;

namespace EventHubConnectorLibrary.Core
{
    public class DeploymentManager
    {
        public static async Task<bool> Deploy(string deployment, CancellationToken cancellationToken)
        {
            var t = Type.GetType(deployment);
            if (t != null)
            {
                return await Deploy(t, cancellationToken);
            }
            return false;
        }

        public static async Task<bool> Deploy(Type deployment, CancellationToken cancellationToken)
        {
            
            var d = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(deployment.Assembly.FullName, deployment.FullName) as IDeployment;
            if (d != null)
            {
                await d.Deploy(cancellationToken);
                return true;
            }
            return false;
        }
    }
}
