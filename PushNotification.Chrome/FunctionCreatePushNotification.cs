
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PushNotification.Chrome
{
    public static class FunctionCreatePushNotification
    {
        [FunctionName("FunctionCreatePushNotification")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var recievingRequestBody = new StreamReader(req.Body).ReadToEnd();

            log.Info($"recievingRequestBody = {recievingRequestBody}");

            var objFromJson = JObject.Parse(recievingRequestBody);
            var payload = objFromJson
                    .SelectToken("payload").ToString();
            var clickTypeName = JObject.Parse(payload)
                    .SelectToken("clickTypeName").ToString();

            log.Info($"clickTypeName = {clickTypeName}");

            var sendingRequest = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            sendingRequest.KeepAlive = true;
            sendingRequest.Method = "POST";
            sendingRequest.ContentType = "application/json; charset=utf-8";

            sendingRequest.Headers.Add("authorization", Variables.OneSignalAuthorizationKey);

            var byteArray = Encoding.UTF8.GetBytes("{"
                                                    + $"\"app_id\": \"{Variables.OneSignalAppId}\","
                                                    + "\"contents\": {\"en\": \"Test notification\", \"ja\": \"プッシュ通知のテストです。\"},"
                                                    + "\"included_segments\": [\"All\"]}");

            var responseContent = string.Empty;

            try
            {
                using (var writer = sendingRequest.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = sendingRequest.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                log.Info(ex.Message);
                log.Info(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            //var requestBody = new StreamReader(req.Body).ReadToEnd();
            //var jo = JObject.Parse(requestBody);
            //responseContent = requestBody;

            //log.Info(jo.ToString());

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
