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
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

using System.Runtime;

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

            NSSet tags = new NSSet("World", "Politics", "Business", "Technology", "Science", "Sports");

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports"};

            //await NativeRegisterWithAzureNotificationHubRegistration(deviceToken, tags);
            await NativeRegisterWithAzureNotificationHubRegistration(deviceToken, stringTags);

            var templates = new JObject();
            templates["genericMessage"] = new JObject
            {
                {"body", templateBodyAPNS}
            };

            //await TemplateRegisterWithAzureNotificationHubRegistration(deviceToken, tags, templates);

            //Hub = new SBNotificationHub(App.ConnectionString, App.NotificationHubName);

            //Hub.UnregisterAllAsync(deviceToken, (error) =>
            //{
                //if (error != null)
                //{
                //    Console.WriteLine("Error calling Unregister: {0}", error.ToString());
                //    return;
                //}

                //NSSet tags = null; // create tags if you want


                //NATIVE REGISTRATION
                //Hub.RegisterNativeAsync(deviceToken, tags, (errorCallback) => {
                //    if (errorCallback != null)
                //        Console.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                //});

                ////TEMPLATE REGISTRATION
                //Hub.RegisterTemplateAsync(deviceToken, templateBodyAPNS, templateBodyAPNS, "0", tags, (errorCallback) =>
                //{
                //    if (errorCallback != null)
                //        Console.WriteLine("RegisterTemplateAsync error: " + errorCallback.ToString());
                //});

                //This will be replaced with a Function calling into a NotificationHub
                //var client = new MobileServiceClient(XamUNotif.App.MobileServiceUrl);
                //await client.GetPush().RegisterAsync(deviceToken, templates);
            //});

            //var myHttpClient = new HttpClient();
            //var stringPayload = JsonConvert.SerializeObject(deviceToken);
            //var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            //var httpRequest = new HttpRequestMessage
            //{
            //    Method = new HttpMethod("PUT"),
            //    RequestUri = new Uri(apiUrl),
            //    Content = httpContent
            //};

            //try
            //{
            //    //UpdateActivityIndicatorStatus(true);

            //    return await myHttpClient.SendAsync(httpRequest).ConfigureAwait(false);
            //}
            //catch (Exception e)
            //{
            //    //AppCenterHelpers.LogException(e);
            //    return null;
            //}
            //finally
            //{
            //    //UpdateActivityIndicatorStatus(false);
            //}

            //CREATE HTTP CLIENT

            //protected static async Task<double> GetAllCosmosPrayerRequests(string apiUrl)
            //{
            //SIMPLIFIED FORM CREATING NEW HTTP CLIENT
            //var myHttpClient = new HttpClient();
            //string clientString = await myHttpClient.GetStringAsync(apiUrl).ConfigureAwait(false);
            //var deserializedListOfCosmosDBPrayerRequests = JsonConvert.DeserializeObject<List<CosmosDBPrayerRequest>>(clientString);
            //return deserializedListOfCosmosDBPrayerRequests;
            //}

            //protected static async Task<HttpResponseMessage> PutCosmosPrayerRequestByAsync(string apiUrl, CosmosDBPrayerRequest data)
            //{


            //var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(deviceToken)).ConfigureAwait(false);
            //var stringPayload = JsonConvert.SerializeObject(deviceToken);
            //var myStringContent = new StringContent("content");
            //var myStringContent = new StringContent("content", Encoding.UTF8, "application/json");

            ////var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                //var httpRequest = new HttpRequestMessage
                //{
                //    Method = new HttpMethod("PUT"),
                //    RequestUri = new Uri(apiUrl),
                //    Content = httpContent
                //};

                //try
                //{
                //    UpdateActivityIndicatorStatus(true);

                //    return await Client.SendAsync(httpRequest).ConfigureAwait(false);
                //}
                //catch (Exception e)
                //{
                //    //AppCenterHelpers.LogException(e);
                //    return null;
                //}
                //finally
                //{
                //    UpdateActivityIndicatorStatus(false);
                //}
            //}



            //CALL FUNCTION1 (PASSING HANDLE AND TAGS) AND INCLUDE "CLEANED" (NO SPACES ALL UPPER CASE) DEVICE TOKEN

            //FUNCTION1
            //--WILL (DELETE EXISTING REGISTRATIONS WITH SAME HANDLE) -- 
            //--CREATE A REGISTRATION WITH REGISTRATION ID + TAG + (NATIVE VS. TAG BASED)
            //--RETURN SUCCESS OR FAILURE
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; } //NSData
            public string[] Tags { get; set; } //NSSet
        }

        public async Task<HttpResponseMessage> 
            NativeRegisterWithAzureNotificationHubRegistration(NSData deviceToken, string[] setOfTags)
        {
            var myHttpClient = new HttpClient();
            string deviceTokenString = deviceToken.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            string MyApiURL = String.Format("https://notificationregistrationviafunctionstwo.azurewebsites.net/api/GetRegistrationIdPassingHandle/{0}", deviceTokenString);

            //var serializedTags = JsonConvert.SerializeObject(setOfTags);
            //var httpContent = new StringContent(serializedTags, Encoding.UTF8, "application/json");

            //int integerOfSetOfTags = setOfTags.Count();

            //string[] setOfTagsStrings = new string[integerOfSetOfTags-1];

            //for (int i = 0; i < integerOfSetOfTags-1; i++)
            //{
            //    setOfTagsString[i] = setOfTags[i].ToString();
            //}

            //foreach(var item in setOfTags)
            //{
            //}

            var _deviceRegistration = new DeviceRegistration() 
            { 
                Platform = "apns",
//                Handle = deviceToken,
                Handle=deviceTokenString,
                Tags = setOfTags
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);
            //var httpContent = new StringContent(_deviceRegistration, Encoding.UTF8, "application/json");
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
                //UpdateActivityIndicatorStatus(true);
                return await myHttpClient.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                //AppCenterHelpers.LogException(e);
                Console.WriteLine("Did not register");
                return null;
            }
            finally
            {
                //UpdateActivityIndicatorStatus(false);
            }

//            YOU WANT TO ADD TAGS
//            var stringPayload = JsonConvert.SerializeObject(deviceToken);
//            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
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
