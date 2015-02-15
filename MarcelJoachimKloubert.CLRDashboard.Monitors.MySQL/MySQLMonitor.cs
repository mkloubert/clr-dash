// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using MarcelJoachimKloubert.CLRDashboard.Extensions;
using MarcelJoachimKloubert.CLRDashboard.Monitoring;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace MarcelJoachimKloubert.CLRDashboard.Monitors.MySQL
{
    /// <summary>
    /// Monitors MySQL servers.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class MySQLMonitor : MonitorBase
    {
        #region Properties (1)

        /// <summary>
        /// Gets the list of connection strings.
        /// </summary>
        public string[] ConnectionStrings
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (2)

        /// <inheriteddoc />
        protected override IEnumerable<IMonitorItem> OnGetItems()
        {
            foreach (var connStr in this.ConnectionStrings)
            {
                var mysqlConnStr = new MySqlConnectionStringBuilder(connStr);

                var newItem = this.CreateItem();
                newItem.Title = string.Format("MySQL '{0}:{1}'",
                                              mysqlConnStr.Server, mysqlConnStr.Port);

                try
                {
                    using (var mysql = new MySqlConnection(mysqlConnStr.ToString()))
                    {
                        mysql.Open();
                    }

                    newItem.State = MonitorState.OK;
                    newItem.Description = "Connection established";
                }
                catch (Exception ex)
                {
                    var innerEx = ex.GetBaseException() ?? ex;

                    newItem.State = MonitorState.Error;
                    newItem.Description = string.Format("[{0}] {1}",
                                                        innerEx.GetType().FullName,
                                                        innerEx.Message);
                }

                yield return newItem;
            }
        }

        /// <inheriteddoc />
        protected override void OnInitialize(IDictionary<string, object> conf)
        {
            var connStrs = new List<string>();
            if (conf.ContainsKey("connections"))
            {
                var conns = (IEnumerable)conf["connections"];
                if (conns != null)
                {
                    connStrs.AddRange(conns.OfType<object>()
                                           .Select(x => (x.AsString() ?? string.Empty).Trim())
                                           .Where(cs => cs != string.Empty)
                                           .Distinct());
                }
            }

            this.ConnectionStrings = connStrs.ToArray();
        }

        #endregion Methods (2)
    }
}