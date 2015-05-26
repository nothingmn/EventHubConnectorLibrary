using EventHubConnectorLibrary.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Core
{
    public class DeploymentManager
    {
        public static async Task<bool> Deploy(string deployment, CancellationToken cancellationToken, string[] args = null)
        {
            var t = Type.GetType(deployment);
            if (t != null)
            {
                return await Deploy(t, cancellationToken, args);
            }
            return false;
        }

        public static async Task<bool> Deploy(Type deployment, CancellationToken cancellationToken, string[] args = null)
        {
            var d = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(deployment.Assembly.FullName, deployment.FullName) as IDeployment;
            if (d != null)
            {
                await d.Deploy(cancellationToken, args);
                return true;
            }
            return false;
        }
    }
}