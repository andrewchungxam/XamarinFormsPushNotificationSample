using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using AndroidConsoleTriggerNotifications;

namespace AndroidConsoleTriggerNotifications
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            //SendNativeNotificationAsyncNativeAndroid();  //(native registration only)
            //SendTemplateNotificationAsync(); //(Template registration only)
            SendTemplateNotificationMultipleAsync(); //Tempalte registration only)

            //USERNAME
            //SendNativeNotificationAsyncNativeAndroidUsername();  //(native registration only)
            //SendTemplateNotificationAsyncUsername(); //(Template registration only)
            //SendTemplateNotificationMultipleAsyncUsername(); //Tempalte registration only)


            System.Threading.Thread.Sleep(30000);  //MAKE SURE TO PAUSE PROGRAM SO NOTIFICATION CAN BE SENT!
            Console.WriteLine("Hello World!");
        }

        private static async void SendNativeNotificationAsyncNativeAndroid()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR ANDROID APPLICATION - YOU USE A NATIVE REGISTRATION
            //

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            var jsonString4 = "{\"data\":{\"message\":\"From your console - native registration\"}}";

            await hubClient.SendGcmNativeNotificationAsync(jsonString4, "World");

            //USE THIS IF YOU HAVE NOT USED TAGS IN YOUR REGISTRATION
            //HOWEVER - THE SAMPLE IN BRANCH 09 + WILL INCLUDE TAGS
            //BE SURE TO DELETE APP FROM PHONE FIRST AND REBUILD TO TRIGGER NEW REGISTRATIONS
            //await hubClient.SendGcmNativeNotificationAsync(jsonString4);

        }

        private static async void SendTemplateNotificationAsync()
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
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
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
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

        //
        //
        //USERNAME
        //
        //
        //
        //
        //USERNAME
        //
        //

        private static async void SendNativeNotificationAsyncNativeAndroidUsername()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR ANDROID APPLICATION - YOU USE A NATIVE REGISTRATION
            //

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            Array.Resize(ref categories, categories.Length + 1);
            var AndroidClientUsername = "AndroidNativeUser101";
            categories[categories.Length - 1] = "username:" + AndroidClientUsername;

            var jsonString4 = "{\"data\":{\"message\":\"From your console - native registration\"}}";

            await hubClient.SendGcmNativeNotificationAsync(jsonString4, "World");

            //USE THIS IF YOU HAVE NOT USED TAGS IN YOUR REGISTRATION
            //HOWEVER - THE SAMPLE IN BRANCH 09 + WILL INCLUDE TAGS
            //BE SURE TO DELETE APP FROM PHONE FIRST AND REBUILD TO TRIGGER NEW REGISTRATIONS
            //await hubClient.SendGcmNativeNotificationAsync(jsonString4);

        }

        private static async void SendTemplateNotificationAsyncUsername()
        {

            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };


            Array.Resize(ref categories, categories.Length + 1);
            var AndroidClientUsername = "AndroidTemplateUser101";
            categories[categories.Length - 1] = "username:" + AndroidClientUsername;

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

        private static async void SendTemplateNotificationMultipleAsyncUsername()
        {
            //
            //THIS WILL ONLY WORK IF IN YOUR IOS APPLICATION - YOU USE A TEMPLATE REGISTRATION
            //


            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                (
                    AzureConstants.AndroidConsoleFullAccessConnectionString,
                    AzureConstants.AndroidConsoleApplicationNotificationHubName,
                    true
                );
            // Create an array of breaking news categories.
            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            Array.Resize(ref categories, categories.Length + 1);
            var AndroidClientUsername = "AndroidTemplateUser101";
            categories[categories.Length - 1] = "username:" + AndroidClientUsername;

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