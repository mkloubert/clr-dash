// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

namespace MarcelJoachimKloubert.CLRDashboard.Monitoring
{
    /// <summary>
    /// Describes a monitor item.
    /// </summary>
    public interface IMonitorItem : IDashboardObject
    {
        #region Properties (5)

        /// <summary>
        /// Gets the description of the current state.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the ID of the item.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the underlying monitor.
        /// </summary>
        IMonitor Monitor { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        MonitorState State { get; }

        /// <summary>
        /// Gets the title of that item.
        /// </summary>
        string Title { get; }

        #endregion Properties (4)
    }
}