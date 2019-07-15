
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace PushNotification.Chrome
{
    public static class FunctionCreatePushNotification
    {
        [FunctionName("FunctionCreatePushNotification")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", Variables.OneSignalAuthorizationKey);

            var byteArray = Encoding.UTF8.GetBytes("{"
                                                    + "\"app_id\": \"" + Variables.OneSignalAppId + "\","
                                                    + "\"contents\": {\"ja\": \"プッシュ通知のテストです。\"},"
                                                    + "\"included_segments\": [\"All\"]}");

            var responseContent = string.Empty;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                log.Info(ex.Message);
                //System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                log.Info(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            log.Info(responseContent);

            //return (ActionResult)new OkObjectResult($"responseContent");
            return (ActionResult)new OkObjectResult(responseContent);

            //string name = req.Query["name"];

            //string requestBody = new StreamReader(req.Body).ReadToEnd();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
