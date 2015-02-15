// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.UI;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// Shows details of a network interface.
    /// </summary>
    public partial class NetInterfaceInfo : Page
    {
        #region Properties (1)

        /// <summary>
        /// Gets the current network interface.
        /// </summary>
        public NetworkInterface NetworkInterface
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = this.Request.QueryString["id"];

            this.NetworkInterface = NetworkInterface.GetAllNetworkInterfaces()
                                                    .SingleOrDefault(ni => ni.Id == id);
        }

        #endregion Methods (1)
    }
}