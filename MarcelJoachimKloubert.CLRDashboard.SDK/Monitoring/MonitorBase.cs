// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.CLRDashboard.Monitoring
{
    /// <summary>
    /// A basic monitor.
    /// </summary>
    public abstract class MonitorBase : DashboardObjectBase, IMonitor
    {
        #region Fields (1)

        private readonly string _ID;

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        protected MonitorBase()
            : this(sync: new object())
        {
        }

        /// <inheriteddoc />
        protected MonitorBase(object sync)
            : base(sync)
        {
            // generate ID
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var blob = Encoding.UTF8.GetBytes(string.Format(@"{0}:{1}.{2}.{3}",
                                                                this.GetType().Assembly.CodeBase,
                                                                this.GetType().Assembly.FullName,
                                                                this.GetType().FullName,
                                                                this.GetHashCode()));

                this._ID = string.Concat(md5.ComputeHash(blob)
                                            .Select(b => b.ToString("x2")));
            }
        }

        #endregion Constructors (2)

        #region Properties (3)

        /// <inheriteddoc />
        public virtual string Id
        {
            get { return this._ID; }
        }

        /// <inheriteddoc />
        public string Name
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        #endregion Properties (3)

        #region Methods (5)

        /// <summary>
        /// Creates a new monitor item.
        /// </summary>
        /// <param name="title">The title for the new item.</param>
        /// <returns>The new item.</returns>
        protected virtual MonitorItem CreateItem(string title = null)
        {
            var result = new MonitorItem(this);
            result.Title = title;

            result.CalculateId();

            return result;
        }

        /// <inheriteddoc />
        public IEnumerable<IMonitorItem> GetItems()
        {
            return this.OnGetItems();
        }

        /// <inheriteddoc />
        public void Initialize(IDictionary<string, object> conf)
        {
            lock (this._SYNC)
            {
                if (this.IsInitialized)
                {
                    throw new InvalidOperationException();
                }

                this.OnInitialize(conf ?? new ExpandoObject());
                this.IsInitialized = true;
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="MonitorBase.GetItems()" /> method.
        /// </summary>
        /// <returns>The list of items.</returns>
        protected abstract IEnumerable<IMonitorItem> OnGetItems();

        /// <summary>
        /// Stores the logic for the <see cref="MonitorBase.Initialize(IDictionary{string, object})" /> method.
        /// </summary>
        /// <param name="conf">The configuration for that monitor.</param>
        protected abstract void OnInitialize(IDictionary<string, object> conf);

        #endregion Methods (5)
    }
}