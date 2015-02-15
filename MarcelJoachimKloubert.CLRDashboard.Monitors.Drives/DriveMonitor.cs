// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using MarcelJoachimKloubert.CLRDashboard.Extensions;
using MarcelJoachimKloubert.CLRDashboard.Monitoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRDashboard.Monitors.Drives
{
    /// <summary>
    /// Monitors one or more drives.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class DriveMonitor : MonitorBase
    {
        #region Properties (1)

        /// <summary>
        /// Gets the list of drives letters to monitor.
        /// </summary>
        public string[] DriveLetters
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (2)

        /// <inheriteddoc />
        protected override IEnumerable<IMonitorItem> OnGetItems()
        {
            var allDrives = DriveInfo.GetDrives();

            foreach (var dl in this.DriveLetters)
            {
                var newItem = this.CreateItem();

                try
                {
                    var drv = allDrives.Single(d =>
                        {
                            try
                            {
                                return d.Name.ToUpper().Trim().StartsWith(dl);
                            }
                            catch
                            {
                                return false;
                            }
                        });

                    newItem.Title = string.Format("Drive '{0}'", drv.Name);

                    var totalSpace = drv.TotalSize;
                    var totalFree = drv.TotalFreeSpace;

                    var percentage = 0d;
                    if (totalSpace != 0)
                    {
                        percentage = (double)totalFree / (double)totalSpace * 100.0;
                    }

                    if (percentage < 5)
                    {
                        // smaller than 5%
                        newItem.State = MonitorState.Error;
                    }
                    else if (percentage < 15)
                    {
                        // smaller than 15%
                        newItem.State = MonitorState.Warning;
                    }
                    else
                    {
                        newItem.State = MonitorState.OK;
                    }

                    newItem.Description = string.Format("There are still {0} of {1} bytes ({2} %) available.",
                                                        totalFree, totalSpace,
                                                        Math.Floor(percentage));
                }
                catch
                {
                    newItem.State = MonitorState.FatalError;
                }

                yield return newItem;
            }
        }

        /// <inheriteddoc />
        protected override void OnInitialize(IDictionary<string, object> conf)
        {
            var drivesLetters = new List<string>();
            if (conf.ContainsKey("drives"))
            {
                var drives = (IEnumerable)conf["drives"];
                if (drives != null)
                {
                    drivesLetters.AddRange(drives.OfType<object>()
                                                 .Select(x => (x.AsString() ?? string.Empty).ToUpper().Trim())
                                                 .Where(dl => dl != string.Empty)
                                                 .Distinct());
                }
            }

            this.DriveLetters = drivesLetters.ToArray();
        }

        #endregion Methods (2)
    }
}