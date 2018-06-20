using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidConsoleTriggerNotifications
{
    public class AzureConstants
    {
        ////STRING PLACEHOLDERS
        //public const string AndroidConsoleFullAccessConnectionString = ;//(YOU OF COURSE NEED TO INCLUDE SEND PERMSSIONS - I'VE SIMPLY ALLOWED ALL 3 TYPES OF PERMISSION, LISTEN, SEND, MANAGE)
        //public const string AndroidConsoleApplicationNotificationHubName = ; // (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE

        public const string AndroidConsoleFullAccessConnectionString = "Endpoint=sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=AndroidConsoleFullAccessPolicy;SharedAccessKey=ECN6mp6F2sdP381kH9AXdbF3Iv/H5uuvcIZSE5B2WT0=";  //(YOU OF COURSE NEED TO INCLUDE SEND PERMSSIONS - I'VE SIMPLY ALLOWED ALL 3 TYPES OF PERMISSION, LISTEN, SEND, MANAGE)
        public const string AndroidConsoleApplicationNotificationHubName = "mysampleandroidnotificationhub";   // (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE


    }
}