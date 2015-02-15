// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System;

namespace MarcelJoachimKloubert.CLRDashboard.Extensions
{
    /// <summary>
    /// Extension methods for values and objects.
    /// </summary>
    static partial class DashboardExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Returns an object or its content as string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="handleDbNullAsNull">
        /// Handle <see cref="DBNull" /> instance as <see langword="null" /> reference or not.
        /// </param>
        /// <returns>The object as string.</returns>
        public static string AsString(this object obj, bool handleDbNullAsNull = true)
        {
            if (obj is string)
            {
                return (string)obj;
            }

            if (handleDbNullAsNull &&
                DBNull.Value.Equals(obj))
            {
                obj = null;
            }

            if (obj == null)
            {
                return null;
            }

            return obj.ToString();
        }

        #endregion Methods (1)
    }
}