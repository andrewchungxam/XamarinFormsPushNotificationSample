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

namespace NotificationViaFunction
{
    public static class NotificationTriggerViaFunction
    {
        [FunctionName("NotificationTriggerViaFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }

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

                //this errors out - as the string type doesn't match registration string jsonString =  "{\"aps\":{\"alert\":\" message to be displayed\",\"sound\":\"default\",\"badge\":1}, \"Data\":{ \"key1\":\"value1\", \"key2\":\"value2\"}";

                string jsonString2 = "{\"aps\":{\"alert\":\"From your console - native registration\"}}";
                await hubClient.SendAppleNativeNotificationAsync(jsonString2, "World");

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

                Dictionary<string, string> templateParams = new Dictionary<string, string>();
                //{
                //    { "messageParam", "Hello World"}
                //};
                templateParams["messageParam"] = "Breaking News!";
                await hubClient.SendTemplateNotificationAsync(templateParams, categories);

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
