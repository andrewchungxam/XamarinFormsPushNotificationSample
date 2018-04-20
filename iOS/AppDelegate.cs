using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;

using Xamarin.Forms;

using System.Threading.Tasks;
using UserNotifications;


using Newtonsoft.Json.Linq;

//using Microsoft.WindowsAzure.MobileServices;


namespace pushsample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
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

            completionHandler(UIBackgroundFetchResult.NoData);
        }

        void PresentNotification(NSDictionary dict)
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
}
