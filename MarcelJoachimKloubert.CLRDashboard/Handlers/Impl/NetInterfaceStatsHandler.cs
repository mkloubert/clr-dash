// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System.Linq;
using System.Net.NetworkInformation;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Receives statistics for a network interface.
    /// </summary>
    public sealed class NetInterfaceStatsHandler : HttpHandlerBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            var id = context.Request.Form["id"];

            var ni = NetworkInterface.GetAllNetworkInterfaces()
                                     .Single(x => x.Id == id);
            if (ni != null)
            {
                var stats = ni.GetIPv4Statistics();
                if (stats != null)
                {
                    dynamic data = new global::System.Dynamic.ExpandoObject();

                    data.bytes = new
                    {
                        received = stats.BytesReceived,
                        sent = stats.BytesSent,
                    };

                    result.data = data;
                }
                else
                {
                    result = CreateJsonObject(code: 2);
                }
            }
            else
            {
                result = CreateJsonObject(code: 1);
            }
        }

        #endregion Methods (1)
    }
}