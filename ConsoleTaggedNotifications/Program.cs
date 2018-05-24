//
//




using System;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;

namespace ConsoleTaggedNotifications
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SendTemplateNotificationAsync();
            //Console.ReadLine();
            Console.WriteLine("Hello World!");
        }

        public static async void SendTemplateNotificationAsync()
        {
            // Define the notification hub.
            NotificationHubClient hub = NotificationHubClient.
            //CreateClientFromConnectionString("<connection string with full access>", "<hub name>");
            CreateClientFromConnectionString
            (
                "Endpoint = sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=CodeRepoFullAccessSignature;SharedAccessKey=cgVY/Y3FJ71SK4LItB+NeY9Q//Vp2N0Mh39/8kZ5hyY=",
                "MySampleNotificationHub"
            );

            // Create an array of breaking news categories.

            var categories = new string[] { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            // Send the notification as a template notification. All template registrations that contain
            // "messageParam" and the proper tags will receive the notifications.
            // This includes APNS, GCM, WNS, and MPNS template registrations.

            Dictionary<string, string> templateParams = new Dictionary<string, string>();

            foreach (var category in categories)
            {
                templateParams["messageParam"] = "Breaking " + category + " News!";
                await hub.SendTemplateNotificationAsync(templateParams, category);
            }
        }
    }
}
