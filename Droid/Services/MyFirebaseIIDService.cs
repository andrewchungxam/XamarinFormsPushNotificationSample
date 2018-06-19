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
            //SendNativeRegistrationToServer(refreshedToken);
            //SendTemplateRegistrationToServer(refreshedToken);
            SendTemplateMultipleRegistrationToServer(refreshedToken);
        }

        void SendNativeRegistrationToServer(string token)
        {
        // Register with Notification Hubs
        hub = new NotificationHub(App.NotificationHubName,
                                      App.ConnectionString, this);

        //var tags = new List<string>() { };
        var tags = new List<string>() { "World", "Politics", "Business", "Technology", "Science", "Sports"};
        var regID = hub.Register(token, tags.ToArray()).RegistrationId;

        Log.Debug(TAG, $"Successful registration of ID {regID}");
        Console.WriteLine(TAG, $"Successful registration of ID {regID}");
        }

        void SendTemplateRegistrationToServer(string token)
        {
            // Register with Notification Hubs
            hub = new NotificationHub(App.NotificationHubName, App.ConnectionString, this);

            //var tags = new List<string>() { };
            var tags = new List<string>() { "World" };

            String templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

            var regID = hub.RegisterTemplate(token, "SimpleGCMTemplate", templateBodyGCM, tags.ToArray()).RegistrationId;

            Log.Debug(TAG, $"Successful registration of ID {regID}");
            Console.WriteLine(TAG, $"Successful registration of ID {regID}");
        }

        void SendTemplateMultipleRegistrationToServer(string token)
        {
            // Register with Notification Hubs
            hub = new NotificationHub(App.NotificationHubName, App.ConnectionString, this);

            //var tags = new List<string>() { };
            var tags = new List<string>() { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            String templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";

            var regID = hub.RegisterTemplate(token, "SimpleGCMTemplate", templateBodyGCM, tags.ToArray()).RegistrationId;

            Log.Debug(TAG, $"Successful registration of ID {regID}");
            Console.WriteLine(TAG, $"Successful registration of ID {regID}");
        }
    }
}
