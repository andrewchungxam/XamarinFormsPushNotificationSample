using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Firebase.Iid;
using Newtonsoft.Json;
using WindowsAzure.Messaging;

using System.Net.Http;
using System.Text;


namespace pushsample.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        //Task<HttpResponseMessage> AndroidNativeRegisterWithAzureNotificationHubRegistration(string token, string[] setOfTags)
        public const string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
        public const string specificAPINotificationRegistration_NativeAndroidUsername = "/api/GetNativeAndroidRegistrationIdPassingHandleUsername/";

        //Task<HttpResponseMessage> TemplateRegisterWithAzureNotificationHubRegistration(string token, string[] setOfTags, string nameOfTemplate, string templates)
        //string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
        public const string specificAPINotificationRegistration_TemplateAndroidUsername = "/api/GetTemplateAndroidRegistrationWithTagsUsername/";


        const string TAG = "MyFirebaseIIDService";
        NotificationHub hub;

        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "FCM token: " + refreshedToken);
            Console.WriteLine(TAG, "FCM token: " + refreshedToken);

            //REGISTRATION FROM APP DIRECTLY
            //SendNativeRegistrationToServer(refreshedToken);
            //SendTemplateRegistrationToServer(refreshedToken);
            //SendTemplateMultipleRegistrationToServer(refreshedToken);

            //FUNCTION REGISTRATION
            //NativeAndroidSendRegistrationToServerAsync(refreshedToken);
            TemplateAndroidSendRegistrationToServerAsyncTemplate(refreshedToken);

            //FUNCTION REGISTRATION WITH USERNAME
            //NativeAndroidSendRegistrationToServerAsyncWithClientUsername(refreshedToken);
            //TemplateAndroidSendRegistrationToServerAsyncTemplateWithClientUsername(refreshedToken);

        }

        void SendNativeRegistrationToServer(string token)
        {
            // Register with Notification Hubs
            hub = new NotificationHub(App.NotificationHubName,
                                          App.ConnectionString, this);

            //var tags = new List<string>() { };
            var tags = new List<string>() { "World", "Politics", "Business", "Technology", "Science", "Sports" };

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

        ///////////////////////// 
        /// 
        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
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

        async Task NativeAndroidSendRegistrationToServerAsync(string token)
        {
            // This is the template/payload used by iOS. It contains the "messageParam"
            // that will be replaced by our service.

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            await AndroidNativeRegisterWithAzureNotificationHubRegistration(token, stringTags);
        }

        public async Task<HttpResponseMessage> AndroidNativeRegisterWithAzureNotificationHubRegistration(string token, string[] setOfTags)
        {
            var myHttpClient = new HttpClient();
            //string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
            //string specificAPINotificationRegistration_NativeAndroid = "/api/GetNativeAndroidRegistrationIdPassingHandle/";
            //string MyApiURL = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_NativeAndroid, token);

            string MyApiURLUsername = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_NativeAndroidUsername, token);

            var _deviceRegistration = new DeviceRegistration()
            {
                Platform = "gcm",
                Handle = token,
                Tags = setOfTags
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);

            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURLUsername),
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




        async Task TemplateAndroidSendRegistrationToServerAsyncTemplate(string token)
        {
            // This is the template/payload used by Android. It contains the "messageParam"
            // that will be replaced by our service.
            String templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";
            String nameOfTemplate = "SimpleGCMTemplate";

            //var tags = new List<string>() { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            //await TemplateRegisterWithAzureNotificationHubRegistration(token, stringTags, templateBodyAPNS);
            await TemplateRegisterWithAzureNotificationHubRegistration(token, stringTags, nameOfTemplate, templateBodyGCM);
        }

        public async Task<HttpResponseMessage>
             TemplateRegisterWithAzureNotificationHubRegistration(string token, string[] setOfTags, string nameOfTemplate, string templates)
        {
            var myHttpClient = new HttpClient();
            //string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
            //string specificAPINotificationRegistration_TemplateAndroid = "/api/GetTemplateAndroidRegistrationWithTags/";
            //string MyApiURLTemplate_1 = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_TemplateAndroid, token);

            string MyApiURLTemplate_1_Username = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_TemplateAndroidUsername, token);

            var _deviceRegistration = new DeviceRegistrationWithTemplate()
            {
                Platform = "gcm",
                Handle = token,
                Tags = setOfTags,
                name = nameOfTemplate,
                jsonBodyTemplates = templates,
                expiryTemplate = "0"
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);
            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");


            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURLTemplate_1_Username),
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

        //
        //
        //
        //
        //ADDING USERNAME FROM CLIENT 
        //
        //
        //
        //

        async Task NativeAndroidSendRegistrationToServerAsyncWithClientUsername(string token)
        {
            // This is the template/payload used by iOS. It contains the "messageParam"
            // that will be replaced by our service.

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            Array.Resize(ref stringTags, stringTags.Length + 1);
            var AndroidClientUsername = "AndroidClientNewUser101";
            stringTags[stringTags.Length - 1] = "username:" + AndroidClientUsername;

            await AndroidNativeRegisterWithAzureNotificationHubRegistrationWithClientUsername(token, stringTags);

        }

        public async Task<HttpResponseMessage> AndroidNativeRegisterWithAzureNotificationHubRegistrationWithClientUsername(string token, string[] setOfTags)
        {
            var myHttpClient = new HttpClient();
            //string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
            //string specificAPINotificationRegistration_NativeAndroid = "/api/GetNativeAndroidRegistrationIdPassingHandle/";
            //string MyApiURL = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_NativeAndroid, token);

            string MyApiURLUsername = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_NativeAndroidUsername, token);

            var _deviceRegistration = new DeviceRegistration()
            {
                Platform = "gcm",
                Handle = token,
                Tags = setOfTags
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);

            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURLUsername),
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


        async Task TemplateAndroidSendRegistrationToServerAsyncTemplateWithClientUsername(string token)
        {
            // This is the template/payload used by Android. It contains the "messageParam"
            // that will be replaced by our service.
            String templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";
            String nameOfTemplate = "SimpleGCMTemplate";

            //var tags = new List<string>() { "World", "Politics", "Business", "Technology", "Science", "Sports" };

            string[] stringTags = new string[] { "Functions", "World", "Politics", "Business", "Technology", "Science", "Sports" };

            Array.Resize(ref stringTags, stringTags.Length + 1);
            var AndroidClientUsername = "AndroidClientNewUser101";
            stringTags[stringTags.Length - 1] = "username:" + AndroidClientUsername;

            await TemplateRegisterWithAzureNotificationHubRegistration(token, stringTags, nameOfTemplate, templateBodyGCM);
        }

        public async Task<HttpResponseMessage>
        TemplateRegisterWithAzureNotificationHubRegistrationWithClientUsername(string token, string[] setOfTags, string nameOfTemplate, string templates)
        {
            var myHttpClient = new HttpClient();
            //string baseNotificationRegistrationURL_Android = "https://xfnotificationfunctions4.azurewebsites.net";
            //string specificAPINotificationRegistration_TemplateAndroid = "/api/GetTemplateAndroidRegistrationWithTags/";
            //string MyApiURLTemplate_1 = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_TemplateAndroid, token);

            string MyApiURLTemplate_1_Username = String.Format("{0}{1}{2}", baseNotificationRegistrationURL_Android, specificAPINotificationRegistration_TemplateAndroidUsername, token);

            var _deviceRegistration = new DeviceRegistrationWithTemplate()
            {
                Platform = "gcm",
                Handle = token,
                Tags = setOfTags,
                name = nameOfTemplate,
                jsonBodyTemplates = templates,
                expiryTemplate = "0"
            };

            var serializedDeviceRegistration = JsonConvert.SerializeObject(_deviceRegistration);
            var httpContent = new StringContent(serializedDeviceRegistration, Encoding.UTF8, "application/json");


            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(MyApiURLTemplate_1_Username),
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


    }
}



