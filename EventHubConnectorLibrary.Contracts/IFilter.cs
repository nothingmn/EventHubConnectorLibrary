using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Contracts
{
    /// <summary>
    /// A whitelist style filter, must be in this property to be allowed through
    /// </summary>
    public interface IFilter
    {
        string[] Filter { get; set; }
    }
}