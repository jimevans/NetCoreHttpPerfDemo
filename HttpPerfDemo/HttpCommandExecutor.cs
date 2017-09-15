using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace HttpPerfDemo
{
    public class HttpCommandExecutor
    {
        private TimeSpan serverResponseTimeout = TimeSpan.FromSeconds(30);
        private Process driverServiceProcess;

        public HttpCommandExecutor()
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 2000;
        }

        public void StartServer()
        {
            this.driverServiceProcess = new Process();
            this.driverServiceProcess.StartInfo.FileName = Path.Combine(this.GetCurrentDirectory(), "chromedriver.exe");
            this.driverServiceProcess.StartInfo.UseShellExecute = false;
            this.driverServiceProcess.Start();
        }

        public void StopServer()
        {
            this.driverServiceProcess.Kill();
        }

        public virtual string MakeHttpRequest(Uri fullUri, string httpMethod, string requestBody)
        {
            string responseString = string.Empty;
            HttpWebRequest request = HttpWebRequest.Create(fullUri) as HttpWebRequest;
            request.Method = httpMethod;
            request.Timeout = (int)this.serverResponseTimeout.TotalMilliseconds;
            request.Accept = "application/json,image/png";
            request.KeepAlive = false;
            request.ServicePoint.ConnectionLimit = 2000;
            if (request.Method == "POST")
            {
                string payload = requestBody;
                byte[] data = Encoding.UTF8.GetBytes(payload);
                request.ContentType = "application/json;charset=utf-8";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
            }

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                webResponse = ex.Response as HttpWebResponse;
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    string timeoutMessage = "The HTTP request to the remote WebDriver server for URL {0} timed out after {1} seconds.";
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, timeoutMessage, request.RequestUri.AbsoluteUri, this.serverResponseTimeout.TotalSeconds), ex);
                }
                else if (ex.Response == null)
                {
                    string nullResponseMessage = "A exception with a null response was thrown sending an HTTP request to the remote WebDriver server for URL {0}. The status of the exception was {1}, and the message was: {2}";
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, nullResponseMessage, request.RequestUri.AbsoluteUri, ex.Status, ex.Message), ex);
                }
            }

            if (webResponse == null)
            {
                throw new Exception("No response from server for url " + request.RequestUri.AbsoluteUri);
            }
            else
            {
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    using (StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        responseString = responseStreamReader.ReadToEnd();

                    }
                }

            }

            return responseString;
        }

        private string GetCurrentDirectory()
        {
            Assembly executingAssembly = typeof(HttpCommandExecutor).Assembly;
            string currentDirectory = Path.GetDirectoryName(executingAssembly.Location);

            // If we're shadow copying, get the directory from the codebase instead
            if (AppDomain.CurrentDomain.ShadowCopyFiles)
            {
                Uri uri = new Uri(executingAssembly.CodeBase);
                currentDirectory = Path.GetDirectoryName(uri.LocalPath);
            }

            return currentDirectory;
        }
    }
}
