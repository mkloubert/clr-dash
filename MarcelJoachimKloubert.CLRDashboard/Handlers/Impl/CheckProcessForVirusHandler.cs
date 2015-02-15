// LICENSE: AGPL 3 - https://www.gnu.org/licenses/agpl-3.0.txt

// s. https://github.com/mkloubert/clr-dash

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.CLRDashboard.Handlers.Impl
{
    /// <summary>
    /// Checks for virus at VirusTotal.com.
    /// </summary>
    public sealed class CheckProcessForVirusHandler : HttpHandlerBase
    {
        #region Methods (3)

        private static dynamic HttpUploadFile(string url, string filePath, string paramName)
        {
            var file = new FileInfo(filePath);

            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            var boundaryBlob = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(boundaryBlob, 0, boundaryBlob.Length);

                var header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n",
                                           paramName, file.Name);

                var headerbytes = Encoding.UTF8.GetBytes(header);
                requestStream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileStream = file.OpenRead())
                {
                    fileStream.CopyTo(requestStream);
                }

                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                requestStream.Write(trailer, 0, trailer.Length);

                requestStream.Close();
            }

            dynamic result;

            var response = request.GetResponse();
            using (var responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    result = MakeJsonObject(reader.ReadToEnd());
                }

                responseStream.Close();
            }

            return result;
        }

        private static dynamic MakeJsonObject(string jsonStr)
        {
            dynamic result = null;

            if (string.IsNullOrWhiteSpace(jsonStr) == false)
            {
                var serializer = new JsonSerializer();
                using (var reader = new StringReader(jsonStr))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        result = serializer.Deserialize<global::System.Dynamic.ExpandoObject>(jsonReader);
                    }
                }
            }

            return result;
        }

        /// <inheriteddoc />
        protected override void OnProcessRequest(HttpContext context, ref dynamic result)
        {
            var apiKey = (DashboardGlobals.GetAppConfigValue("ApiKey_VirusTotalCom") ?? string.Empty).Trim();
            if (apiKey == string.Empty)
            {
                // no API key

                result = CreateJsonObject(code: 1);
                return;
            }

            try
            {
                int pid;
                if (int.TryParse(context.Request.Form["pid"], out pid))
                {
                    var process = Process.GetProcesses().SingleOrDefault(p => p.Id == pid);
                    if (process != null)
                    {
                        string filePath = null;
                        try
                        {
                            filePath = process.MainModule.FileName;
                        }
                        catch
                        {
                            filePath = null;
                        }

                        if (string.IsNullOrWhiteSpace(filePath) == false)
                        {
                            var file = new FileInfo(filePath);

                            if (file.Exists)
                            {
                                string hash;
                                using (var sha256 = new SHA256CryptoServiceProvider())
                                {
                                    using (var stream = file.OpenRead())
                                    {
                                        hash = string.Concat(sha256.ComputeHash(stream)
                                                                   .Select(b => b.ToString("x2")));
                                    }
                                }

                                var request = (HttpWebRequest)WebRequest.Create("https://www.virustotal.com/vtapi/v2/file/rescan");
                                request.Method = "POST";
                                request.KeepAlive = true;

                                var postData = string.Format("resource={0}", HttpUtility.UrlEncode(hash)) +
                                               string.Format("&apikey={0}", HttpUtility.UrlEncode(apiKey));

                                var binPostData = Encoding.ASCII.GetBytes(postData);

                                request.ContentLength = binPostData.Length;

                                using (var requestStream = request.GetRequestStream())
                                {
                                    requestStream.Write(binPostData, 0, binPostData.Length);

                                    requestStream.Close();
                                }

                                var response = request.GetResponse();
                                using (var responseStream = response.GetResponseStream())
                                {
                                    string jsonStr;
                                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                                    {
                                        jsonStr = reader.ReadToEnd();
                                    }

                                    dynamic jsonResult = MakeJsonObject(jsonStr);
                                    if (jsonResult != null)
                                    {
                                        if (jsonResult.response_code == 1)
                                        {
                                            // no need to scan

                                            result.data = new
                                            {
                                                link = jsonResult.permalink,
                                            };
                                        }
                                        else if (jsonResult.response_code == 0)
                                        {
                                            // not in database => scan

                                            if (file.Length <= (128 * 1024 * 1024))
                                            {
                                                // maximum size: 128 MB

                                                var jsonResult2 = HttpUploadFile("https://www.virustotal.com/vtapi/v2/file/scan",
                                                                                 file.FullName,
                                                                                 "file");

                                                if (jsonResult2.response_code == 1)
                                                {
                                                    result.data = new
                                                    {
                                                        link = jsonResult.permalink,
                                                    };
                                                }
                                                else
                                                {
                                                    // unknown response
                                                    result = CreateJsonObject(code: 10,
                                                                              data: new
                                                                              {
                                                                                  code = jsonResult2.response_code,
                                                                              });
                                                }
                                            }
                                            else
                                            {
                                                // file too big
                                                result = CreateJsonObject(code: 9);
                                            }
                                        }
                                        else
                                        {
                                            // unknown response
                                            result = CreateJsonObject(code: 8,
                                                                      data: new
                                                                      {
                                                                          code = jsonResult.response_code,
                                                                      });
                                        }
                                    }
                                    else
                                    {
                                        // no result
                                        result = CreateJsonObject(code: 7);
                                    }

                                    responseStream.Close();
                                }
                            }
                            else
                            {
                                // file not found
                                result = CreateJsonObject(code: 6);
                            }
                        }
                        else
                        {
                            // no file
                            result = CreateJsonObject(code: 5);
                        }
                    }
                    else
                    {
                        // process not found
                        result = CreateJsonObject(code: 4);
                    }
                }
                else
                {
                    // invalid process ID
                    result = CreateJsonObject(code: 3);
                }
            }
            catch (Exception ex)
            {
                // exception
                result = CreateJsonObject(code: 2,
                                          data: ToJsonObject(ex.GetBaseException() ?? ex));
            }
        }

        #endregion Methods (3)
    }
}