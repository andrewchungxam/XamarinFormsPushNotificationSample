//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ConsoleApplicationToTriggerNotifications
//{
//    public class AzureConstants
//    {
//        ////STRING PLACEHOLDERS
//        //public const string ConsoleApplicationFullAccessConnectionString = ;  (YOU OF COURSE NEED TO INCLUDE SEND PERMSSIONS - I'VE SIMPLY ALLOWED ALL 3 TYPES OF PERMISSION, LISTEN, SEND, MANAGE)
//        //public const string ConsoleApplicationNotificationHubName = ;    (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE

//    }
//}

using System;
using System.Collections.Generic;
using System.Text;

namespace AzureConstants
{
    public class AzureConstants
    {
        ////STRING PLACEHOLDERS
        //public const string ConsoleApplicationFullAccessConnectionString = ;
        //public const string ConsoleApplicationNotificationHubName = ;

        //STRINGS
        public const string ConsoleApplicationFullAccessConnectionString = "Endpoint=sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=ConsoleApplicationFullAccess;SharedAccessKey=DivOPPwrOoRsXhdDpTGBLE+iW4COfUeX4OY96yRVX5Y=";
        public const string ConsoleApplicationNotificationHubName = "MySampleNotificationHub";
    }
}