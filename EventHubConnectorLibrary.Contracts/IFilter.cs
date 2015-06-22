using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubConnectorLibrary.Contracts
{
    public interface IFilter
    {
        string Filter { get; set; }
    }
}