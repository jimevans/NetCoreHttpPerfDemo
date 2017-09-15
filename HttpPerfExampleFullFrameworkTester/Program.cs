using HttpPerfDemo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpPerfExampleFullFrameworkTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting test...");
            HttpCommandExecutor executor = new HttpCommandExecutor();

            executor.StartServer();
            string sessionId = StartSession(executor);
            NavigateToUrl(executor, sessionId, "http://www.google.com");

            Console.WriteLine("Making 10 HTTP calls to localhost, logging the elapsed time...");
            for (int i = 0; i < 10; i++)
            {
                GetPageTitle(executor, sessionId);
            }

            Console.WriteLine("Test finished. Press <Enter> to exit.");
            Console.ReadLine();
            StopSession(executor, sessionId);
            executor.StopServer();
        }

        private static void NavigateToUrl(HttpCommandExecutor executor, string sessionId, string url)
        {
            Console.WriteLine("Navigating to {0}", url);
            Uri startSessionUri = new Uri(string.Format("http://localhost:9515/session/{0}/url", sessionId));
            string response = executor.MakeHttpRequest(startSessionUri, "POST", string.Format("{{ \"url\": \"{0}\" }}", url));
            Console.WriteLine("Navigation complete");
        }

        private static void GetPageTitle(HttpCommandExecutor executor, string sessionId)
        {
            Uri startSessionUri = new Uri(string.Format("http://localhost:9515/session/{0}/title", sessionId));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string response = executor.MakeHttpRequest(startSessionUri, "GET", string.Empty);
            stopwatch.Stop();
            Console.WriteLine("Elapsed time for HTTP call: {0} milliseconds", stopwatch.ElapsedMilliseconds);
        }

        private static void StopSession(HttpCommandExecutor executor, string sessionId)
        {
            Console.WriteLine("Ending session {0}", sessionId);
            Uri startSessionUri = new Uri(string.Format("http://localhost:9515/session/{0}", sessionId));
            string response = executor.MakeHttpRequest(startSessionUri, "DELETE", string.Empty);
            Console.WriteLine(response);
        }

        private static string StartSession(HttpCommandExecutor executor)
        {
            Uri startSessionUri = new Uri("http://localhost:9515/session");
            string rawResponse = executor.MakeHttpRequest(startSessionUri, "POST", "{\"desiredCapabilities\": { \"browserName\": \"chrome\" } }");
            Response response = JsonConvert.DeserializeObject<Response>(rawResponse);
            Console.WriteLine("Started session {0}", response.SessionId);
            return response.SessionId;
        }
    }
}
