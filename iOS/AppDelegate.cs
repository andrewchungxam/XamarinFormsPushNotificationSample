using System;
using System.Text;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UIKit;
using Foundation;

using Xamarin.Forms;
using UserNotifications;
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

            //await SendRegistrationToServerAsyncNative(deviceToken);
            await SendRegistrationToServerAsyncTemplate(deviceToken);
        }

        async Task SendRegistrationToServerAsyncNative(NSData deviceToken)
        {
            // This is the template/payload used by iOS. It contains the "messageParam"
            // that will be replaced by our service.



            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            //registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            //registration.Tags.Add("username:" + "friendlyUser101");

            stringTags.Append<string>("username:" + "friendlyUser101");
            var hi = stringTags;


            await NativeRegisterWithAzureNotificationHubRegistration(deviceToken, stringTags);

            //NSSet tags = new NSSet("World", "Politics", "Business", "Technology", "Science", "Sports");
            //await NativeRegisterWithAzureNotificationHubRegistration(deviceToken, tags);
        }

        async Task SendRegistrationToServerAsyncTemplate(NSData deviceToken)
        {
            // This is the template/payload used by iOS. It contains the "messageParam"
            // that will be replaced by our service.
            const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

            //NSSet tags = new NSSet("World", "Politics", "Business", "Technology", "Science", "Sports");

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            var templates = new JObject();
            templates["genericMessage"] = new JObject
            {
                {"body", templateBodyAPNS}
            };

            await TemplateRegisterWithAzureNotificationHubRegistration(deviceToken, stringTags, templateBodyAPNS);
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; } //NSData
            public string[] Tags { get; set; } //NSSet
        }

        public class DeviceRegistrationWithTemplate
        {
            public string Platform { get; set; }
            public string Handle { get; set; } //NSData
            public string[] Tags { get; set; } //NSSet
            public string name { get; set; }
            public string jsonBodyTemplates { get; set; }
            public string expiryTemplate { get; set; }
        }

        public async Task<HttpResponseMessage> 
            NativeRegisterWithAzureNotificationHubRegistration(NSData deviceToken, string[] setOfTags)
        {
            var myHttpClient = new HttpClient();
            string deviceTokenString = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            string MyApiURL = String.Format("https://notificationregistrationviafunctionstwo.azurewebsites.net/api/GetRegistrationIdPassingHandle/{0}", deviceTokenString);

            var _deviceRegistration = new DeviceRegistration() 
            { 
                Platform = "apns",
                Handle=deviceTokenString,
                Tags = setOfTags
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);

            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURL),
                Content = httpContent
            };

            Console.WriteLine("{0}", httpRequest.RequestUri.ToString());

            try
            {
                return await myHttpClient.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                //AppCenterHelpers.LogException(e);
                Console.WriteLine("Did not register");
                return null;
            }
        }

        public async Task<HttpResponseMessage>
        TemplateRegisterWithAzureNotificationHubRegistration(NSData deviceToken, string[] setOfTags, string templates)
        {
            var myHttpClient = new HttpClient();
            string deviceTokenString = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            string MyApiURLTemplate = String.Format("https://notificationregistrationviafunctionstwo.azurewebsites.net/api/GetTemplateRegistrationWithTags/{0}", deviceTokenString);

            var _deviceRegistration = new DeviceRegistrationWithTemplate()
            {
                Platform = "apns",
                Handle = deviceTokenString,
                Tags = setOfTags,
                jsonBodyTemplates = templates
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);
            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURLTemplate),
                Content = httpContent
            };

            Console.WriteLine("{0}", httpRequest.RequestUri.ToString());

            try
            {
                return await myHttpClient.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                //AppCenterHelpers.LogException(e);
                Console.WriteLine("Did not register");
                return null;
            }
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
    }
}
