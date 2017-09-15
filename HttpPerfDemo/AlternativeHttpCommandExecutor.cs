using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

#if NETSTANDARD2_0
using System.Net.Http;
#endif

namespace HttpPerfDemo
{
    public class AlternativeHttpCommandExecutor : HttpCommandExecutor
    {
#if NETSTANDARD2_0
        private HttpClient client = new HttpClient();
#endif

        public AlternativeHttpCommandExecutor()
        {
        }

        public override string MakeHttpRequest(Uri fullUri, string httpMethod, string requestBody)
        {
            string responseString = string.Empty;
#if NETSTANDARD2_0
            HttpMethod httpMethodValue = HttpMethod.Get;
            if (httpMethod == "POST")
            {
                httpMethodValue = HttpMethod.Post;
            }
            else if (httpMethod == "DELETE")
            {
                httpMethodValue = HttpMethod.Delete;
            }

            HttpRequestMessage message = new System.Net.Http.HttpRequestMessage(httpMethodValue, fullUri);
            message.Headers.Accept.Clear();
            message.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            message.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/png"));
            message.Content = new System.Net.Http.StringContent(requestBody, Encoding.UTF8);
            HttpResponseMessage responseMessage = this.client.SendAsync(message, System.Net.Http.HttpCompletionOption.ResponseHeadersRead).Result;
            return responseString = responseMessage.Content.ReadAsStringAsync().Result;
#else
            throw new Exception("Must use .NET Core 2.0 with AlternateHttpCommandExecutor");
#endif
        }
    }
}
