// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Receives the icon of a process.
    /// </summary>
    public sealed class GetProcessIcon : HttpHandlerBase
    {
        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            Image icon = null;

            try
            {
                int id;
                if (int.TryParse(context.Request.Form["pid"], out id))
                {
                    var process = Process.GetProcesses().FirstOrDefault(p => p.Id == id);
                    if (process != null)
                    {
                        using (var i = Icon.ExtractAssociatedIcon(process.MainModule.FileName))
                        {
                            icon = i.ToBitmap();
                        }
                    }
                }
            }
            catch
            {
                // error
            }

            if (icon == null)
            {
                return;
            }

            try
            {
                using (icon)
                {
                    using (var temp = new MemoryStream())
                    {
                        icon.Save(temp, ImageFormat.Png);

                        result.data = new
                            {
                                data = temp.ToArray(),
                                mime = "image/png",
                            };
                    }
                }
            }
            catch
            {
                // ignore here
            }
        }

        #endregion Methods (1)
    }
}