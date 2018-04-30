using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using UIKit;
using Foundation;

using Xamarin.Forms;
using UserNotifications;

using Newtonsoft.Json.Linq;
using WindowsAzure.Messaging;


namespace pushsample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        private SBNotificationHub Hub { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            RequestPushPermissionAsync();

            _launchOptions = options;

            return base.FinishedLaunching(app, options);
        }

        NSDictionary _launchOptions;

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            // If app was not running and we come from a notificatio badge, the notification is delivered via the options.
            if (_launchOptions != null && _launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
            {
                var notification = _launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                PresentNotification(notification);
            }
            _launchOptions = null;
        }




        async Task RequestPushPermissionAsync()
        {
            // iOS10 and later (https://developer.xamarin.com/guides/ios/platform_features/user-notifications/enhanced-user-notifications/#Preparing_for_Notification_Delivery)
            // Register for ANY type of notification (local or remote):
            var requestResult = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert
                | UNAuthorizationOptions.Badge
                | UNAuthorizationOptions.Sound);


            // Item1 = approved boolean
            bool approved = requestResult.Item1;
            NSError error = requestResult.Item2;
            if (error == null)
            {
                // Handle approval
                if (!approved)
                {
                    Console.Write("Permission to receive notifications was not granted.");
                    return;
                }

                var currentSettings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
                if (currentSettings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
                {
                    Console.WriteLine("Permissions were requested in the past but have been revoked (-> Settings app).");
                    return;
                }

                UIApplication.SharedApplication.RegisterForRemoteNotifications();

                //VIA DOCS
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);

            }
            else
            {
                Console.Write($"Error requesting permissions: {error}.");
            }
        }

        public async override void RegisteredForRemoteNotifications(
            UIApplication application, NSData deviceToken)
        {
            if (deviceToken == null)
            {
                // Can happen in rare conditions e.g. after restoring a device.
                return;
            }

            Console.WriteLine($"Token received: {deviceToken}");
            await SendRegistrationToServerAsync(deviceToken);
        }

        async Task SendRegistrationToServerAsync(NSData deviceToken)
        {
            // This is the template/payload used by iOS. It contains the "messageParam"
            // that will be replaced by our service.
            const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

            var templates = new JObject();
            templates["genericMessage"] = new JObject
            {
                {"body", templateBodyAPNS}
            };


            Hub = new SBNotificationHub(App.ConnectionString, App.NotificationHubName);

            Hub.UnregisterAllAsync(deviceToken, (error) => {
                if (error != null)
                {
                    Console.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }

                NSSet tags = null; // create tags if you want
                Hub.RegisterNativeAsync(deviceToken, tags, (errorCallback) => {
                    if (errorCallback != null)
                        Console.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                });
            });


            //This will be replaced with a Function calling into a NotificationHub
            //var client = new MobileServiceClient(XamUNotif.App.MobileServiceUrl);
            //await client.GetPush().RegisterAsync(deviceToken, templates);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.WriteLine($"Failed to register for remote notifications: {error.Description}");
        }

        public override void DidReceiveRemoteNotification(
            UIApplication application,
            NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            // This will be called if the app is in the background/not running and if in the foreground.
            // However, it will not display a notification visually if the app is in the foreground.

            PresentNotification(userInfo);
            //ProcessNotification(userInfo, false);

            completionHandler(UIBackgroundFetchResult.NoData);
        }

        void PresentNotification(NSDictionary dict)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != dict && dict.ContainsKey(new NSString("aps")))
            {
                // Extract some data from the notifiation and display it using an alert view.
                NSDictionary aps = dict.ObjectForKey(new NSString("aps")) as NSDictionary;

                var msg = string.Empty;
                if (aps.ContainsKey(new NSString("alert")))
                {
                    msg = (aps[new NSString("alert")] as NSString).ToString();
                }

                if (string.IsNullOrEmpty(msg))
                {
                    msg = "(unable to parse)";
                }

                MessagingCenter.Send<object, string>(this, App.NotificationReceivedKey, msg);
            }
        }

        //void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
        //{
        //    // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
        //    if (null != options && options.ContainsKey(new NSString("aps")))
        //    {
        //        //Get the aps dictionary
        //        NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

        //        string alert = string.Empty;

        //        //Extract the alert text
        //        // NOTE: If you're using the simple alert by just specifying
        //        // "  aps:{alert:"alert msg here"}  ", this will work fine.
        //        // But if you're using a complex alert with Localization keys, etc.,
        //        // your "alert" object from the aps dictionary will be another NSDictionary.
        //        // Basically the JSON gets dumped right into a NSDictionary,
        //        // so keep that in mind.
        //        if (aps.ContainsKey(new NSString("alert")))
        //            alert = (aps[new NSString("alert")] as NSString).ToString();

        //        //If this came from the ReceivedRemoteNotification while the app was running,
        //        // we of course need to manually process things like the sound, badge, and alert.
        //        if (!fromFinishedLaunching)
        //        {
        //            //Manually show an alert
        //            if (!string.IsNullOrEmpty(alert))
        //            {
        //                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
        //                avAlert.Show();
        //            }
        //        }
        //    }

        //    NSDictionary aps1 = options.ObjectForKey(new NSString("aps")) as NSDictionary;

        //    var msg = string.Empty;
        //    if (aps1.ContainsKey(new NSString("alert")))
        //    {
        //        msg = (aps1[new NSString("alert")] as NSString).ToString();
        //    }

        //    if (string.IsNullOrEmpty(msg))
        //    {
        //        msg = "(unable to parse)";
        //    }

        //    MessagingCenter.Send<object, string>(this, App.NotificationReceivedKey, msg);

        //}
    }
}
