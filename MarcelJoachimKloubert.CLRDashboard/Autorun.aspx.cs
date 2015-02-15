// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Collections.Generic;
using System.Management;
using System.Web.UI;
using System.Linq;
using MarcelJoachimKloubert.CLRDashboard.Extensions;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// Handles Autorun entries.
    /// </summary>
    public partial class Autorun : Page
    {
        #region CLASS: AutorunEntry

        /// <summary>
        /// Stores data of an autorun entry.
        /// </summary>
        public sealed class AutorunEntry
        {
            #region Properties (6)
            
            /// <summary>
            /// Gets or sets the caption.
            /// </summary>
            public string Caption { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            public string Command { get; set; }
            
            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the location in the registry.
            /// </summary>
            public string Location { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the user.
            /// </summary>
            public string User { get; set; }

            #endregion
        }

        #endregion

        #region Properties (1)

        public AutorunEntry[] AutorunEntries
        {
            get;
            private set;
        }

        #endregion

        #region Methods (1)

        /// <inheriteddoc />
        protected void Page_Load(object sender, EventArgs e)
        {
            var entries = new List<AutorunEntry>();

            using (var mangnmt = new ManagementClass("Win32_StartupCommand"))
            {
                using (var coll = mangnmt.GetInstances())
                {
                    foreach (ManagementObject obj in coll)
                    {
                        var newEntry = new AutorunEntry()
                        {
                            Caption = obj["Caption"].AsString(),
                            Command = obj["Command"].AsString(),
                            Description = obj["Description"].AsString(),
                            Location = obj["Location"].AsString(),
                            Name = obj["Name"].AsString(),
                            User = obj["User"].AsString(),
                        };

                        entries.Add(newEntry);
                    }
                }
            }

            this.AutorunEntries = entries.OrderBy(entry => entry.Caption, StringComparer.InvariantCultureIgnoreCase)
                                         .ThenBy(entry => entry.Name, StringComparer.InvariantCultureIgnoreCase)
                                         .ThenBy(entry => entry.Command, StringComparer.InvariantCultureIgnoreCase)
                                         .ThenBy(entry => entry.Location, StringComparer.InvariantCultureIgnoreCase)
                                         .ToArray();
        }

        #endregion Methods (1)
    }
}