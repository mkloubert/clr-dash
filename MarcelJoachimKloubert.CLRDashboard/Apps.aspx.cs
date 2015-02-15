// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;
using System.Web.UI;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// Lists app information.
    /// </summary>
    public partial class Apps : Page
    {
        #region Methods (2)

        /// <summary>
        /// Gets a value without throwing an exception.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="func">The function that provides the value.</param>
        /// <param name="defValue">The default value if providing value fails.</param>
        /// <returns>The value.</returns>
        public static T GetValue<T>(Func<T> func, T defValue = default(T))
        {
            try
            {
                return func();
            }
            catch
            {
                return defValue;
            }
        }

        /// <inheriteddoc />
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion Methods (2)
    }
}