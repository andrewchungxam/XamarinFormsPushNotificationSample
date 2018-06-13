using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

using AzureConstants;
using Newtonsoft.Json;
using SharedClasses;

namespace pushsample
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //SendNativeNotificationAsyncNativeApple();  //(native registration only)
            //SendTemplateNotificationAsync(); //(Template registration only)
            SendTemplateNotificationMultipleAsync(); //Tempalte registration only)

            System.Threading.Thread.Sleep(30000);  //MAKE SURE TO PAUSE PROGRAM SO NOTIFICATION CAN BE SENT!
            Console.WriteLine("Hello World!");
        }



        private static async void SendNativeNotificationAsyncNativeApple()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A NATIVE REGISTRATION
            //


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
            var sdf = categories[1];

            //this errors out - as the string type doesn't match registration string jsonString =  "{\"aps\":{\"alert\":\" message to be displayed\",\"sound\":\"default\",\"badge\":1}, \"Data\":{ \"key1\":\"value1\", \"key2\":\"value2\"}";
            for (int i = 0; i < categories.Length; i++)
            {
                //FOR FORMAT REQUIRING MODIFIED MESSAGES IN YOUR ALERT
                var _apns = new Aps() {
                    alert = String.Format("From your console - {0} native registration.", categories[i])
                };

                var _rootObject = new RootObject {
                    aps = _apns
                };

                string newJsonString = JsonConvert.SerializeObject(_rootObject);

                await hubClient.SendAppleNativeNotificationAsync(newJsonString, categories[i]);

                //FOR FORMATS NOT REQUIRING MODIFIED MESSAGES IN YOUR ALERT
                //string jsonString2 = String.Format("{\"aps\":{\"alert\":\"From your console - native registration\"}}");
                //await hubClient.SendAppleNativeNotificationAsync(jsonString2, categories[i]);
            }
        }

        private static async void SendTemplateNotificationAsync()
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

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
        }

        private static async void SendTemplateNotificationMultipleAsync()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //


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
            //templateParams["messageParam"] = "Breaking News!";

            foreach (var category in categories)
            {
                templateParams["messageParam"] = "Breaking " + category + " News!";
                await hubClient.SendTemplateNotificationAsync(templateParams, categories);
            }
        }
    }
}