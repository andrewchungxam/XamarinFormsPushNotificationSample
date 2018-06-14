using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using ConsoleApplicationToTriggerNotifications;

namespace pushsample
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            //SendTemplateNotificationAsyncNativeApple();  //(native registration only)
            //SendTemplateNotificationAsync(); //(Template registration only)
            SendTemplateNotificationMultipleAsync(); //Tempalte registration only)

            System.Threading.Thread.Sleep(30000);  //MAKE SURE TO PAUSE PROGRAM SO NOTIFICATION CAN BE SENT!
            Console.WriteLine("Hello World!");
        }

        private static async void SendTemplateNotificationAsyncNativeApple()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A NATIVE REGISTRATION
            //


            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.ConsoleApplicationFullAccessConnectionString,
                    AzureConstants.ConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            //this errors out - as the string type doesn't match registration string jsonString =  "{\"aps\":{\"alert\":\" message to be displayed\",\"sound\":\"default\",\"badge\":1}, \"Data\":{ \"key1\":\"value1\", \"key2\":\"value2\"}";

            string jsonString2 = "{\"aps\":{\"alert\":\"From your console - native registration\"}}";
            await hubClient.SendAppleNativeNotificationAsync(jsonString2, "World");
        }

        private static async void SendTemplateNotificationAsync()
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.ConsoleApplicationFullAccessConnectionString,
                    AzureConstants.ConsoleApplicationNotificationHubName,
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
        }

        private static async void SendTemplateNotificationMultipleAsync()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //


            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.ConsoleApplicationFullAccessConnectionString,
                    AzureConstants.ConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

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