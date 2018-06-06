using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure.NotificationHubs;

using PushSampleWebAPIBackend.AzureConstants;

namespace PushSampleWebAPIBackend.Models
{
    public class Notifications
    {

        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString
                (
                    PushSampleWebAPIBackend.AzureConstants.AzureConstants.ConsoleApplicationFullAccessConnectionString,
                    PushSampleWebAPIBackend.AzureConstants.AzureConstants.ConsoleApplicationNotificationHubName
                );
        }

    }
}