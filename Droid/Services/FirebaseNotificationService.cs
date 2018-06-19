using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using Xamarin.Forms;
using static Android.OS.Build;

namespace pushsample.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                Console.WriteLine(TAG, "Notification Message Body: " + message.GetNotification().Body);

                SendNotification(message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                SendNotification(message.Data.Values.First());

            }

        }

        void SendNotification(string messageBody)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationManager = NotificationManager.FromContext(this);

            string NotificationChannelIdentifier = "my_channel_101";

            if (VERSION.SdkInt >= Build.VERSION_CODES.O)
            {
                //NotificationChannel channel = new NotificationChannel("notify_001","Channel human readable title", NotificationManager.ImportanceDefault);
                NotificationChannel channel = new NotificationChannel(NotificationChannelIdentifier, "Channel human readable title", Android.App.NotificationImportance.High); //NotificationManager.ImportanceDefault);
                channel.EnableLights(true);
                channel.EnableVibration(true);
                channel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
                channel.Description = "Great description";
                channel.LightColor = Android.Graphics.Color.AliceBlue;
                notificationManager.CreateNotificationChannel(channel);

            }

            var notificationBuilder = new NotificationCompat.Builder(this, NotificationChannelIdentifier)
            .SetSmallIcon(Resource.Drawable.ic_launcher)
            .SetContentTitle("New Notification Message")
            .SetContentText(messageBody)
            .SetContentIntent(pendingIntent)
            .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
            .SetAutoCancel(true);

            notificationManager.Notify(0, notificationBuilder.Build());

            MessagingCenter.Send<object, string>(this, App.NotificationReceivedKey, messageBody);
        }
    }
}


//void SendNotification(string messageBody)
//{
//    var intent = new Intent(this, typeof(MainActivity));
//    intent.AddFlags(ActivityFlags.ClearTop);
//    var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

//    var notificationBuilder = new Notification.Builder(this)
//                .SetContentTitle("FCM Message")
//                .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
//                .SetContentText(messageBody)
//                .SetAutoCancel(true)
//                .SetContentIntent(pendingIntent);

//    var notificationManager = NotificationManager.FromContext(this);

//    notificationManager.Notify(0, notificationBuilder.Build());
//}


//    [Service]
//    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
//    public class FirebaseNotificationService : FirebaseMessagingService
//    {
//        const string TAG = "FirebaseNotificationService";

//        //TEMPLATE STYLE
//        //public override void OnMessageReceived(RemoteMessage message)
//        //{
//        //    Log.Debug(TAG, "From: " + message.From);

//        //    // Pull message body out of the template
//        //    var messageBody = message.Data["message"];
//        //    if (string.IsNullOrWhiteSpace(messageBody))
//        //        return;

//        //    Log.Debug(TAG, "Notification message body: " + messageBody);
//        //    SendNotification(messageBody);
//        //}

//        public override void OnMessageReceived(RemoteMessage message)
//        {
//            Log.Debug(TAG, "From: " + message.From);
//            if (message.GetNotification() != null)
//            {
//                //These is how most messages will be received
//                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
//                SendNotification(message.GetNotification().Body);
//            }
//            else
//            {
//                //Only used for debugging payloads sent from the Azure portal
//                SendNotification(message.Data.Values.First());
//            }
//        }

//        //void SendNotification(string messageBody)
//        //{
//            //var intent = new Intent(this, typeof(MainActivity));
//            //intent.AddFlags(ActivityFlags.ClearTop);
//            //var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

//            //var notificationBuilder = new NotificationCompat.Builder(this)
//                //.SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
//                //.SetContentTitle("New Todo Item")
//                //.SetContentText(messageBody)
//                //.SetContentIntent(pendingIntent)
//                //.SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
//                //.SetAutoCancel(true);

//            //var notificationManager = NotificationManager.FromContext(this);
//            //notificationManager.Notify(0, notificationBuilder.Build());
//        //}

//        void SendNotification(string messageBody)
//        {
//            var intent = new Intent(this, typeof(MainActivity));
//            intent.AddFlags(ActivityFlags.ClearTop);
//            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

//            var notificationBuilder = new Notification.Builder(this)
//                        .SetContentTitle("FCM Message")
//                        .SetSmallIcon(Resource.Drawable.ic_launcher)
//                        .SetContentText(messageBody)
//                        .SetAutoCancel(true)
//                        .SetContentIntent(pendingIntent);

//            var notificationManager = NotificationManager.FromContext(this);

//            notificationManager.Notify(0, notificationBuilder.Build());
//        }
//    }
//}
