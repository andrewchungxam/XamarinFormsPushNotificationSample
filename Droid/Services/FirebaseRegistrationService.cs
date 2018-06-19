using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Firebase.Iid;
using WindowsAzure.Messaging;

namespace pushsample.Droid.Services
{
        [Service]
        [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
        public class MyFirebaseIIDService : FirebaseInstanceIdService
        {

        const string TAG = "MyFirebaseIIDService";
        NotificationHub hub;

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "FCM token: " + refreshedToken);
            Console.WriteLine(TAG, "FCM token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
        // Register with Notification Hubs
        hub = new NotificationHub(App.NotificationHubName,
                                      App.ConnectionString, this);

        var tags = new List<string>() { };
        var regID = hub.Register(token, tags.ToArray()).RegistrationId;

        Log.Debug(TAG, $"Successful registration of ID {regID}");
        Console.WriteLine(TAG, $"Successful registration of ID {regID}");
        }
    }
}

//    [Service]
//    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
//    public class FirebaseRegistrationService : FirebaseInstanceIdService
//    {
//        const string TAG = "FirebaseRegistrationService";

//        NotificationHub hub;

//        public override void OnTokenRefresh()
//        {
//            var refreshedToken = FirebaseInstanceId.Instance.Token;
//            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
//            Console.Write(TAG, "Refreshed token: " + refreshedToken);
//            SendRegistrationTokenToAzureNotificationHub(refreshedToken);
//        }

//        void SendRegistrationTokenToAzureNotificationHub(string token)
//        {
//            // Update notification hub registration
//            //Task.Run(async () =>
//            //{
//            //    await AzureNotificationHubService.RegisterAsync(TodoItemManager.DefaultManager.CurrentClient.GetPush(), token);
//            //});

//            hub = new NotificationHub(App.NotificationHubName,
//                                      App.ConnectionString, this);

//            var tags = new List<string>() { };
//            var regID = hub.Register(token, tags.ToArray()).RegistrationId;

//            Log.Debug(TAG, $"Successful registration of ID {regID}");
//        }
//    }
//}



