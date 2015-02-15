// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// Page that shows process details.
    /// </summary>
    public partial class ProcessDetails : Page
    {
        #region Properties (1)

        /// <summary>
        /// Gets the current underlying process.
        /// </summary>
        public Process CurrentProcess
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (2)

        /// <summary>
        /// Returns a value of the current process.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="func">The function that provides the value.</param>
        /// <param name="defValue">The default value if an error occures.</param>
        /// <returns>The value.</returns>
        public T GetProcessValue<T>(Func<Process, T> func, T defValue = default(T))
        {
            try
            {
                return func(this.CurrentProcess);
            }
            catch
            {
                return defValue;
            }
        }

        /// <inheriteddoc />
        protected void Page_Load(object sender, EventArgs e)
        {
            int pid;
            if (int.TryParse(this.Request.QueryString["pid"], out pid))
            {
                this.CurrentProcess = Process.GetProcesses()
                                             .FirstOrDefault(p => p.Id == pid);
            }
        }

        #endregion Methods (2)
    }
}