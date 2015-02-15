// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Receives memory information.
    /// </summary>
    public sealed class MemoryInfoHandler : HttpHandlerBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            var info = new ComputerInfo();

            dynamic data = new global::System.Dynamic.ExpandoObject();

            data.available = new
            {
                physical = info.AvailablePhysicalMemory,
                @virtual = info.AvailableVirtualMemory,
            };

            data.total = new
            {
                physical = info.TotalPhysicalMemory,
                @virtual = info.TotalVirtualMemory,
            };

            result.data = data;
        }

        #endregion Methods (1)
    }
}