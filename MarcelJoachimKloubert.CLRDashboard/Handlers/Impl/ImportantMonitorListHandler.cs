// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using MarcelJoachimKloubert.CLRDashboard.Monitoring;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Receives the current list of important monitors and their items (warnings, errors, etc.).
    /// </summary>
    public sealed class ImportantMonitorListHandler : HttpHandlerBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            var monitors = new List<dynamic>();
            foreach (var m in DashboardGlobals.GetMonitors())
            {
                dynamic newEntry = new global::System.Dynamic.ExpandoObject();

                newEntry.name = m.Name;
                if (string.IsNullOrWhiteSpace(newEntry.name))
                {
                    newEntry.name = m.GetType().FullName;
                }

                var items = new List<dynamic>();
                try
                {
                    var mItems = (m.GetItems() ?? Enumerable.Empty<IMonitorItem>()).OfType<IMonitorItem>();
                    foreach (var i in mItems)
                    {
                        dynamic newItem = new global::System.Dynamic.ExpandoObject();
                        try
                        {
                            newItem.state = (int)i.State;
                        }
                        catch
                        {
                            newItem.state = (int)MonitorState.FatalError;
                        }

                        if (newItem.state != (int)MonitorState.OK)
                        {
                            newItem.title = i.Title;
                            newItem.description = i.Description;

                            items.Add(newItem);
                        }
                    }

                    newEntry.state = (int)MonitorState.OK;
                }
                catch
                {
                    newEntry.state = (int)MonitorState.FatalError;
                }

                if ((items.Count > 0) ||
                    (newEntry.state != (int)MonitorState.OK))
                {
                    newEntry.items = items;

                    monitors.Add(newEntry);
                }
            }

            result.data = monitors;
        }

        #endregion Methods (1)
    }
}