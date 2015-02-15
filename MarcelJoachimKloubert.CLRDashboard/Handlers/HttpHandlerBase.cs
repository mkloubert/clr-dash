// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers
{
    /// <summary>
    /// A basic HTTP handler that outputs JSON objects.
    /// </summary>
    public abstract class HttpHandlerBase : IHttpHandler, IRouteHandler, IRequiresSessionState
    {
        #region Fields (1)

        /// <summary>
        /// Stores the object for handling thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHandlerBase" /> class.
        /// </summary>
        protected HttpHandlerBase()
            : this(sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpHandlerBase" /> class.
        /// </summary>
        /// <param name="sync">The value for the <see cref="HttpHandlerBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected HttpHandlerBase(object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._SYNC = sync;
        }

        #endregion Constructors (2)

        #region Properties (2)

        /// <inheriteddoc />
        public virtual bool IsReusable
        {
            get { return false; }
        }

        /// <inheriteddoc />
        public virtual IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        #endregion Properties (2)

        #region Methods (5)

        /// <summary>
        /// Creates a new synamic JSON object.
        /// </summary>
        /// <param name="code">The value for the code property.</param>
        /// <param name="msg">The value for the msg property.</param>
        /// <param name="data">The value for the data property.</param>
        /// <returns>The created object.</returns>
        protected static dynamic CreateJsonObject(int? code, string msg = null, object data = null)
        {
            dynamic result = new global::System.Dynamic.ExpandoObject();
            result.code = code;
            result.msg = msg;
            result.data = null;

            return result;
        }

        /// <summary>
        /// The logic that sets up the result object for an error / exception.
        /// </summary>
        /// <param name="ex">The occured exception.</param>
        /// <param name="result">The result object.</param>
        protected virtual void OnError(Exception ex, ref dynamic result)
        {
            result = CreateJsonObject(-1,
                                      "SERVER ERROR",
                                      ToJsonObject(ex));
        }

        /// <summary>
        /// The logic for the <see cref="HttpHandlerBase.ProcessRequest(HttpContext)" /> method.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="result">The result object.</param>
        protected abstract void OnProcessRequest(HttpContext context, ref dynamic result);

        /// <inheriteddoc />
        public void ProcessRequest(HttpContext context)
        {
            var utf8 = Encoding.UTF8;

            dynamic result = null;

            try
            {
                result = CreateJsonObject(0);

                this.OnProcessRequest(context, ref result);
            }
            catch (Exception ex)
            {
                this.OnError(ex, ref result);
            }

            var serializer = new JsonSerializer();
            using (var strWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(strWriter))
                {
                    jsonWriter.CloseOutput = false;

                    serializer.Serialize(jsonWriter, result);
                }

                var dataToSend = utf8.GetBytes(strWriter.ToString());

                context.Response.ContentType = "application/json; charset=" + utf8.WebName;
                context.Response
                       .OutputStream
                       .Write(dataToSend, 0, dataToSend.Length);
            }
        }

        /// <summary>
        /// Creates a JSON object from an exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The JSON object.</returns>
        protected static object ToJsonObject(Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            return new
            {
                innerEx = ToJsonObject(ex.InnerException),
                msg = ex.Message,
                type = ex.GetType().FullName,
            };
        }

        #endregion Methods (4)
    }
}