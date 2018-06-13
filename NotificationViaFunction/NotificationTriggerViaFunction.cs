using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using System.Collections.Generic;
using System.Diagnostics;
using System;
using SharedClasses;
using Newtonsoft.Json;

namespace NotificationViaFunction
{
    public static class NotificationTriggerViaFunction
    {
        //PLEASE AVOID PORT EXHAUSTION: 
        //https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections

        //[FunctionName("NotificationTriggerViaFunction")]
        //public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        //{
        //    log.Info("C# HTTP trigger function processed a request.");

        //    // parse query parameter
        //    string name = req.GetQueryNameValuePairs()
        //        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        //        .Value;

        //    // Get request body
        //    dynamic data = await req.Content.ReadAsAsync<object>();

        //    // Set name to query string or body data
        //    name = name ?? data?.name;

        //    return name == null
        //        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        //        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        //}

        [FunctionName("NotificationFunctionNative")]
        public static async Task<HttpResponseMessage> RunNotificationFunctionNative([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A NATIVE REGISTRATION
            //

            try
            { 
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AzureConstants.AzureConstants.ConsoleApplicationFullAccessConnectionString,
                        AzureConstants.AzureConstants.ConsoleApplicationNotificationHubName,
                        true
                    );
                // Create an array of breaking news categories.
                var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

                Array.Resize(ref categories, categories.Length + 1);
                var nativeUsername = "NativeUser101";
                categories[categories.Length - 1] = "username:" + nativeUsername;                

                for (int i = 0; i < categories.Length; i++)
                {
                    //FOR FORMAT REQUIRING MODIFIED MESSAGES IN YOUR ALERT
                    var _apns = new Aps()
                    {
                        alert = String.Format("From Azure Functions - {0} native registration.", categories[i])
                    };

                    var _rootObject = new RootObject
                    {
                        aps = _apns
                    };

                    string newJsonString = JsonConvert.SerializeObject(_rootObject);

                    await hubClient.SendAppleNativeNotificationAsync(newJsonString, categories[i]);

                    //FOR FORMATS NOT REQUIRING MODIFIED MESSAGES IN YOUR ALERT
                    //string jsonString2 = String.Format("{\"aps\":{\"alert\":\"From your console - native registration\"}}");
                    //await hubClient.SendAppleNativeNotificationAsync(jsonString2, categories[i]);
                }

                return req.CreateResponse(System.Net.HttpStatusCode.OK, "Success - Notification Function triggered - Native");
            }

            catch (Exception exception)
            {
                Debug.WriteLine("Error: ", exception.Message);
            }

            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Bad Requestion - Notification Function triggered - Native");
        }


        [FunctionName("NotificationFunctionTemplate")]
        public static async Task<HttpResponseMessage> RunNotificationFunctionTemplate([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

            try
            {
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AzureConstants.AzureConstants.ConsoleApplicationFullAccessConnectionString,
                        AzureConstants.AzureConstants.ConsoleApplicationNotificationHubName,
                        true
                    );
                // Create an array of breaking news categories.
                var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

                Array.Resize(ref categories, categories.Length + 1);
                var templateUsername = "TemplateUser101";
                categories[categories.Length - 1] = "username:" + templateUsername;

                Dictionary<string, string> templateParams = new Dictionary<string, string>();
                //{
                //    { "messageParam", "Hello World"}
                //};
                //SINGLE NOTIFICATION
                templateParams["messageParam"] = "Breaking " + categories[0] + " News!";
                await hubClient.SendTemplateNotificationAsync(templateParams, "World");

                //MULTIPLE NOTIFICATIONS
                //await hubClient.SendTemplateNotificationAsync(templateParams, categories);

                return req.CreateResponse(System.Net.HttpStatusCode.OK, "Success - Notification Function triggered - Template");

            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error: ", exception.Message);
            }

            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Bad Requestion - Notification Function triggered - Template");
        }

        [FunctionName("NotificationFunctionTemplateMultiple")]
        public static async Task<HttpResponseMessage> RunNotificationFunctionTemplateMultiple([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //
            try
            {
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AzureConstants.AzureConstants.ConsoleApplicationFullAccessConnectionString,
                        AzureConstants.AzureConstants.ConsoleApplicationNotificationHubName,
                        true
                    );
                // Create an array of breaking news categories.
                var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

                Array.Resize(ref categories, categories.Length + 1);
                var templateUsername = "TemplateUser101";
                categories[categories.Length - 1] = "username:" + templateUsername;

                Dictionary<string, string> templateParams = new Dictionary<string, string>();

                foreach (var category in categories)
                {
                    templateParams["messageParam"] = "Breaking " + category + " News!";
                    await hubClient.SendTemplateNotificationAsync(templateParams, categories);
                }

                return req.CreateResponse(System.Net.HttpStatusCode.OK, "Success - Notification Function triggered - Template Multiple");

            }

            catch (Exception exception)
            {
                Debug.WriteLine("Error: ", exception.Message);
            }

            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Bad Requestion - Notification Function triggered - Template Multiple");
        }

    }
}
