// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using MarcelJoachimKloubert.CLRDashboard.Extensions;
using MarcelJoachimKloubert.CLRDashboard.Monitoring;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard
{
    /// <summary>
    /// Global access of services and functions.
    /// </summary>
    public static class DashboardGlobals
    {
        #region Fields (4)

        /// <summary>
        /// Stores the function / method that provides the application configuration.
        /// </summary>
        public static Func<Configuration> AppConfigProvider;

        /// <summary>
        /// Handler that provides all avaiable monitors.
        /// </summary>
        public static Func<IEnumerable<IMonitor>> MonitorProvider;

        /// <summary>
        /// Stores the global service catalog.
        /// </summary>
        public static AggregateCatalog ServiceCatalog;

        /// <summary>
        /// Stores the global composition container.
        /// </summary>
        public static CompositionContainer ServiceContainer;

        #endregion Fields (4)

        #region Methods (1´3)

        /// <summary>
        /// Returns all instance of a specific type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="key">The optional key to use.</param>
        /// <returns>The instances.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<object> GetAllInstances(Type serviceType, object key = null)
        {
            return InvokeGetExportedValueMethod(c => c.GetExportedValues<object>(),
                                                ServiceContainer,
                                                serviceType,
                                                key.AsString());
        }

        /// <summary>
        /// Returns all instance of a specific type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="key">The optional key to use.</param>
        /// <returns>The instances.</returns>
        public static IEnumerable<T> GetAllInstances<T>(object key = null)
        {
            return GetAllInstances(typeof(T), key).Cast<T>();
        }

        /// <summary>
        /// Returns the current app configuration.
        /// </summary>
        /// <returns>The current app configuration.</returns>
        public static Configuration GetAppConfig()
        {
            return AppConfigProvider();
        }

        /// <summary>
        /// Returns a value from the app settings.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="name">The name of the setting.</param>
        /// <param name="beforeReturn">
        /// Optional logic to invoke BEFORE setting value is converted.
        /// </param>
        /// <returns>The value.</returns>
        public static T GetAppConfigValue<T>(string name,
                                             Func<string, string, object> beforeConvert = null)
        {
            var targetType = typeof(T);

            var value = GetAppConfigValue(name);

            object objToConvert;
            if (beforeConvert != null)
            {
                objToConvert = beforeConvert(name, value);
            }
            else
            {
                objToConvert = value;
            }

            if (objToConvert == null)
            {
                var nullableType = Nullable.GetUnderlyingType(targetType);
                if (nullableType != null)
                {
                    // nullable struct
                    return default(T);
                }
                else
                {
                    // create default instance of target type
                    return (T)(targetType.IsValueType ? Activator.CreateInstance(targetType)
                                                      : null);
                }
            }

            return (T)Convert.ChangeType(objToConvert,
                                         targetType);
        }

        /// <summary>
        /// Returns a value from the app settings.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="beforeReturn">
        /// Optional logic to invoke BEFORE setting value is returned.
        /// </param>
        /// <returns>The value.</returns>
        public static string GetAppConfigValue(string name,
                                               Func<string, string, object> beforeReturn = null)
        {
            var value = AppConfigProvider().AppSettings.Settings[name].Value;
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            object result;
            if (beforeReturn != null)
            {
                result = beforeReturn(name, value);
            }
            else
            {
                result = value;
            }

            return result.AsString();
        }

        /// <summary>
        /// Returns the full path of the config directory.
        /// </summary>
        /// <returns>The config directory.</returns>
        public static string GetConfigDirectory()
        {
            return Path.Combine(GetRootDirectory(), "conf");
        }

        /// <summary>
        /// Returns an instance of a specific type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="key">The optional key to use.</param>
        /// <returns>The instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        public static object GetInstance(Type serviceType, object key = null)
        {
            return InvokeGetExportedValueMethod(c => c.GetExportedValue<object>(),
                                                ServiceContainer,
                                                serviceType,
                                                key.AsString());
        }

        /// <summary>
        /// Returns an instance of a specific type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="key">The optional key to use.</param>
        /// <returns>The instance.</returns>
        public static T GetInstance<T>(object key = null)
        {
            return (T)GetInstance(typeof(T), key);
        }

        /// <summary>
        /// Returns the monitor directory.
        /// </summary>
        /// <returns>The full path of the monitor directory.</returns>
        public static string GetMonitorDirectory()
        {
            return Path.Combine(GetRootDirectory(), "monitors");
        }

        /// <summary>
        /// Returns all avaiable monitors.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IMonitor> GetMonitors()
        {
            return MonitorProvider();
        }

        /// <summary>
        /// Returns the root directory.
        /// </summary>
        /// <returns>The full path of the root directory.</returns>
        public static string GetRootDirectory()
        {
            return HttpContext.Current.Server.MapPath("~");
        }

        private static R InvokeGetExportedValueMethod<R>(Expression<Func<CompositionContainer, R>> expr,
                                                         CompositionContainer container,
                                                         Type serviceType,
                                                         string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            var method = (expr.Body as MethodCallExpression).Method;

            var @params = new object[0];
            if (key != null)
            {
                @params = new object[] { key };
            }

            return (R)ServiceContainer.GetType()
                                      .GetMethods()
                                      .First(m =>
                                             {
                                                 if (m.Name != method.Name)
                                                 {
                                                     return false;
                                                 }

                                                 if (m.IsGenericMethod == false)
                                                 {
                                                     return false;
                                                 }

                                                 return m.GetParameters().Length ==
                                                        @params.Length;
                                             }).MakeGenericMethod(serviceType)
                                      .Invoke(obj: ServiceContainer,
                                              parameters: @params);
        }

        /// <summary>
        /// Loads JSON configuration.
        /// </summary>
        /// <param name="name">The name of the config file (NOT the path).</param>
        /// <returns>The loaded data.</returns>
        public static IDictionary<string, object> LoadConfig(string name)
        {
            ExpandoObject result = null;

            var file = new FileInfo(Path.Combine(GetConfigDirectory(), name + ".json"));
            if (file.Exists)
            {
                var json = File.ReadAllText(file.FullName, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(json) == false)
                {
                    var serializer = new JsonSerializer();
                    using (var reader = new StringReader(json))
                    {
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            jsonReader.CloseInput = false;

                            result = serializer.Deserialize<global::System.Dynamic.ExpandoObject>(jsonReader);
                        }
                    }
                }
            }

            return result ?? new ExpandoObject();
        }

        #endregion Methods (1´3)
    }
}