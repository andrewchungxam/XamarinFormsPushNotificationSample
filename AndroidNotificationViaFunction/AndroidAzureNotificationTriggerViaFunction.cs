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

namespace AndroidNotificationViaFunction
    {
    public static class AndroidAzureNotificationTriggerViaFunction
        {

        [FunctionName("AndroidNotificationFunctionNative")]
        public static async Task<HttpResponseMessage> RunAndroidNotificationFunctionNative([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A NATIVE REGISTRATION
            //

            try
            {
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AndroidAzureConstants.AzureConstants.AndroidConsoleFullAccessConnectionString,
                        AndroidAzureConstants.AzureConstants.AndroidConsoleApplicationNotificationHubName,
                        true
                    );
                // Create an array of breaking news categories.
                var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

                var jsonString2 = "{\"data\":{\"message\":\"From your console - native registration\"}}";

                await hubClient.SendGcmNativeNotificationAsync(jsonString2, "World");

                return req.CreateResponse(System.Net.HttpStatusCode.OK, "Success - Notification Function triggered - Native");
            }

            catch (Exception exception)
            {
                Debug.WriteLine("Error: ", exception.Message);
            }

            return req.CreateResponse(System.Net.HttpStatusCode.BadRequest, "Bad Requestion - Notification Function triggered - Native");


        }


        [FunctionName("AndroidNotificationFunctionTemplate")]
        public static async Task<HttpResponseMessage> RunAndroidNotificationFunctionTemplate([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

            try
            {
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AndroidAzureConstants.AzureConstants.AndroidConsoleFullAccessConnectionString,
                        AndroidAzureConstants.AzureConstants.AndroidConsoleApplicationNotificationHubName,
                        true
                    );
                // Create an array of breaking news categories.
                var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

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

        [FunctionName("AndroidNotificationFunctionTemplateMultiple")]
        public static async Task<HttpResponseMessage> RunAndroidNotificationFunctionTemplateMultiple([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //
            try
            {
                NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AndroidAzureConstants.AzureConstants.AndroidConsoleFullAccessConnectionString,
                        AndroidAzureConstants.AzureConstants.AndroidConsoleApplicationNotificationHubName,
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
