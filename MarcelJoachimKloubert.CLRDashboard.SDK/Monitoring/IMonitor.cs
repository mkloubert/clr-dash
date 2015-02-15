// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRDashboard.Monitoring
{
    /// <summary>
    /// Describes a monitor.
    /// </summary>
    public interface IMonitor : IDashboardObject
    {
        #region Properties (3)

        /// <summary>
        /// Gets the ID of the monitor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets the name of the monitor.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets if the monitor has been initialized or not.
        /// </summary>
        bool IsInitialized { get; }

        #endregion Properties (3)

        #region Methods (2)

        /// <summary>
        /// Returns all items.
        /// </summary>
        /// <returns>The list of items.</returns>
        IEnumerable<IMonitorItem> GetItems();

        /// <summary>
        /// Initializes the monitor.
        /// </summary>
        /// <param name="conf">The configuration for the monitor.</param>
        /// <exception cref="InvalidOperationException">Monitor has already been initialized.</exception>
        void Initialize(IDictionary<string, object> conf);

        #endregion Methods (2)
    }
}