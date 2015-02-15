// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// A basic object.
    /// </summary>
    public abstract class DashboardObjectBase : IDashboardObject
    {
        #region Fields (1)

        /// <summary>
        /// Stores the object for handling thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardObjectBase" /> class.
        /// </summary>
        protected DashboardObjectBase()
            : this(sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardObjectBase" /> class.
        /// </summary>
        /// <param name="sync">The value for the <see cref="DashboardObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected DashboardObjectBase(object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._SYNC = sync;
        }

        #endregion Constructors (2)
    }
}