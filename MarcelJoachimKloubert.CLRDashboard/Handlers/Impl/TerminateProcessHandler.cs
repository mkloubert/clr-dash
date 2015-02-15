// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Web;
using System.Linq;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Terminates a process.
    /// </summary>
    public sealed class TerminateProcessHandler : HttpHandlerBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            try
            {
                int id;
                if (int.TryParse(context.Request.Form["pid"], out id))
                {
                    var name = context.Request.Form["pname"];

                    var process = Process.GetProcesses().FirstOrDefault(p => p.Id == id &&
                                                                             p.ProcessName == name);
                    if (process != null)
                    {
                        process.Kill();
                    }
                    else
                    {
                        // not found
                        result = CreateJsonObject(code: 3);
                    }
                }
                else
                {
                    // invalid value
                    result = CreateJsonObject(code: 2);
                }
            }
            catch
            {
                // error
                result = CreateJsonObject(code: 1);
            }
        }

        #endregion Methods (1)
    }
}