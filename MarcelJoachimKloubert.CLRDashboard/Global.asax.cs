// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using MarcelJoachimKloubert.CLRDashboard.Extensions;
using MarcelJoachimKloubert.CLRDashboard.Monitoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Routing;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// The global application class.
    /// </summary>
    public class Global : global::System.Web.HttpApplication
    {
        #region Fields (1)

        private IEnumerable<IMonitor> _monitors;

        #endregion Fields (1)

        #region Methods (11)

        /// <inheriteddoc />
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_End(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_Error(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_Start(object sender, EventArgs e)
        {
            DashboardGlobals.AppConfigProvider = GetWebConfig;

            // MEF
            DashboardGlobals.ServiceCatalog = new AggregateCatalog();
            DashboardGlobals.ServiceContainer = new CompositionContainer(catalog: DashboardGlobals.ServiceCatalog,
                                                                         isThreadSafe: true);

            // monitors
            this.InitMonitors();
            DashboardGlobals.MonitorProvider = this.GetMonitors;

            // HTTP handlers
            InitHttpHandlers(RouteTable.Routes);
        }

        private IEnumerable<IMonitor> GetMonitors()
        {
            return this._monitors;
        }

        private static Configuration GetWebConfig()
        {
            return WebConfigurationManager.OpenWebConfiguration("~/");
        }

        private static void InitHttpHandlers(RouteCollection routes)
        {
            routes.Add(new Route("actions/check_process_for_virus",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.CheckProcessForVirusHandler()));

            routes.Add(new Route("actions/terminate_process",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.TerminateProcessHandler()));

            routes.Add(new Route("info/cpu",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.CpuInfoHandler()));

            routes.Add(new Route("info/mem",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.MemoryInfoHandler()));

            routes.Add(new Route("lists/important_monitors",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.ImportantMonitorListHandler()));

            routes.Add(new Route("res/process_icon",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.GetProcessIcon()));

            routes.Add(new Route("stats/net_interface",
                                 new global::MarcelJoachimKloubert.CLRDashboard.Handlers.Impl.NetInterfaceStatsHandler()));
        }

        private void InitMonitors()
        {
            var conf = DashboardGlobals.LoadConfig("monitors");

            var monitorDir = new DirectoryInfo(DashboardGlobals.GetMonitorDirectory());
            if (monitorDir.Exists)
            {
                foreach (var dll in monitorDir.GetFiles("*.dll"))
                {
                    var asm = Assembly.LoadFile(dll.FullName);

                    DashboardGlobals.ServiceCatalog
                                    .Catalogs.Add(new AssemblyCatalog(asm));
                }
            }

            var monitors = new List<IMonitor>();

            if (conf.ContainsKey("monitors"))
            {
                var items = conf["monitors"] as global::System.Collections.IEnumerable;
                if (items != null)
                {
                    foreach (IDictionary<string, object> i in items)
                    {
                        if (i.ContainsKey("name") == false)
                        {
                            continue;
                        }

                        var monitorTypeName = (i["name"].AsString() ?? string.Empty).Trim();
                        if (monitorTypeName == string.Empty)
                        {
                            continue;
                        }

                        var monitorType = DashboardGlobals.ServiceCatalog
                                                          .Catalogs
                                                          .OfType<AssemblyCatalog>()
                                                          .Select(ac => ac.Assembly)
                                                          .SelectMany(a => a.GetTypes())
                                                          .Single(t => t.FullName == monitorTypeName);

                        var newMonitor = (IMonitor)DashboardGlobals.GetInstance(monitorType);

                        if (i.ContainsKey("displayText"))
                        {
                            newMonitor.Name = i["displayText"].AsString();
                        }

                        if (newMonitor.IsInitialized == false)
                        {
                            IDictionary<string, object> monitorConf = null;
                            if (i.ContainsKey("config"))
                            {
                                monitorConf = (IDictionary<string, object>)i["config"];
                            }

                            newMonitor.Initialize(monitorConf);
                        }

                        if (newMonitor.IsInitialized)
                        {
                            monitors.Add(newMonitor);
                        }
                    }
                }
            }

            this._monitors = monitors;
        }

        /// <inheriteddoc />
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        #endregion Methods (11)
    }
}