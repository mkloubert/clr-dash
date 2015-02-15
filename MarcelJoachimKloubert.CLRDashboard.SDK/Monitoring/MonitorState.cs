// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

namespace MarcelJoachimKloubert.CLRDashboard.Monitoring
{
    /// <summary>
    /// List of monitor states.
    /// </summary>
    public enum MonitorState
    {
        /// <summary>
        /// None / gray
        /// </summary>
        None = 0,

        /// <summary>
        /// OK / green
        /// </summary>
        OK = 1,

        /// <summary>
        /// Warning / yellow
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error / red
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal error / yellow font with red background
        /// </summary>
        FatalError = 4,
    }
}