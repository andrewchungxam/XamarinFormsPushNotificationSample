using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidAzureConstants
{
    public class AndroidAzureConstants
    {
        ////STRING PLACEHOLDERS
        //public const string ConsoleApplicationFullAccessConnectionString = ;
        //public const string ConsoleApplicationNotificationHubName = ;

        //STRINGS

        //        public const string AndroidConsoleApplicationNotificationHubName = "mysampleandroidnotificationhub";   // (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE
        public const string AndroidConsoleFullAccessConnectionString = "Endpoint=sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=AndroidConsoleFullAccessPolicy;SharedAccessKey=ECN6mp6F2sdP381kH9AXdbF3Iv/H5uuvcIZSE5B2WT0=";  //(YOU OF COURSE NEED TO INCLUDE SEND PERMSSIONS - I'VE SIMPLY ALLOWED ALL 3 TYPES OF PERMISSION, LISTEN, SEND, MANAGE)
        public const string AndroidConsoleApplicationNotificationHubName = "mysampleandroidnotificationhub";   // (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE


    }

    public class AzureNotificationsViaFunctionsURLS
    {
        public const string NativeApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/NotificationFunctionNative";
        public const string TemplateApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/NotificationFunctionTemplate";
        public const string MultipleTemplateApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/NotificationFunctionTemplateMultiple";

        public const string AndroidNativeApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/AndroidNotificationFunctionNative";
        public const string AndroidTemplateApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/AndroidNotificationFunctionTemplate";
        public const string AndroidMultipleTemplateApiURL = "https://xamarinformsnotificationfunction.azurewebsites.net/" + "api/AndroidNotificationFunctionTemplateMultiple";


    }
}




//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace AndroidAzureConstants
//{
//    public class AzureConstants
//    {
//        ////STRING PLACEHOLDERS
//        //public const string AndroidConsoleFullAccessConnectionString = ;
//        //public const string AndroidConsoleApplicationNotificationHubName = ;

//        //STRINGS
//        public const string AndroidConsoleFullAccessConnectionString = "Endpoint=sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=AndroidConsoleFullAccessPolicy;SharedAccessKey=ECN6mp6F2sdP381kH9AXdbF3Iv/H5uuvcIZSE5B2WT0=";  //(YOU OF COURSE NEED TO INCLUDE SEND PERMSSIONS - I'VE SIMPLY ALLOWED ALL 3 TYPES OF PERMISSION, LISTEN, SEND, MANAGE)
//        public const string AndroidConsoleApplicationNotificationHubName = "mysampleandroidnotificationhub";   // (THIS IS JUST THE NAME OF THE NOTIFICATION HUB -- DO NOT USE THE NOTIFICATION NAMESPACE

//    }
//}
