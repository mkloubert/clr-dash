// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System.Diagnostics;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Receives CPU information.
    /// </summary>
    public sealed class CpuInfoHandler : HttpHandlerBase
    {
        #region Fields (1)

        private readonly PerformanceCounter _CPU_TIME_COUNTER;

        #endregion Fields (1)

        #region Constructors (1)

        public CpuInfoHandler()
        {
            this._CPU_TIME_COUNTER = new PerformanceCounter();
            this._CPU_TIME_COUNTER.CategoryName = "Processor";
            this._CPU_TIME_COUNTER.CounterName = "% Processor Time";
            this._CPU_TIME_COUNTER.InstanceName = "_Total";
            this._CPU_TIME_COUNTER.ReadOnly = true;
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            dynamic data = new global::System.Dynamic.ExpandoObject();

            data.usage = this._CPU_TIME_COUNTER.NextValue();

            result.data = data;
        }

        #endregion Methods (1)
    }
}