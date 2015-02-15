// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.CLRDashboard.Monitoring
{
    /// <summary>
    /// Simple implementation of the <see cref="IMonitorItem" /> interface.
    /// </summary>
    public sealed class MonitorItem : DashboardObjectBase, IMonitorItem
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorItem" /> class.
        /// </summary>
        /// <param name="monitor">The value for the <see cref="MonitorItem.Monitor" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="monitor" /> is <see langword="null" />.
        /// </exception>
        public MonitorItem(IMonitor monitor)
        {
            if (monitor == null)
            {
                throw new ArgumentNullException("monitor");
            }

            this.Monitor = monitor;
        }

        #endregion Constructors (1)

        #region Properties (4)

        /// <inheriteddoc />
        public string Description
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public string Id
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IMonitor Monitor
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public MonitorState State
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public string Title
        {
            get;
            set;
        }

        #endregion Properties (4)

        #region Methods (1)

        /// <summary>
        /// Generates a value for the <see cref="MonitorItem.Id" /> propety.
        /// </summary>
        public void CalculateId()
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var blob = Encoding.UTF8.GetBytes(string.Format(@"{0}:{1}.{2}.{3}.{4}",
                                                                this.Monitor.GetType().Assembly.CodeBase,
                                                                this.Monitor.GetType().Assembly.FullName,
                                                                this.Monitor.GetType().FullName,
                                                                this.Monitor.GetHashCode(),
                                                                (this.Title ?? string.Empty).ToUpper().Trim()));

                this.Id = string.Concat(md5.ComputeHash(blob)
                                           .Select(b => b.ToString("x2")));
            }
        }

        #endregion Methods (1)
    }
}