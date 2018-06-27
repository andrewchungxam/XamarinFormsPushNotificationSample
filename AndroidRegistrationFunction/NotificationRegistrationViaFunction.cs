using System.Linq;
using System.Net;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;

using AndroidAzureConstants;

namespace AndroidRegistrationFunction
{
    public static class NotificationRegistrationViaFunction
    {

        //PLEASE AVOID PORT EXHAUSTION: 
        //https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections

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
            public string jsonBodyTemplates { get; set; }
            public string expiryTemplate { get; set; }
            public string name { get; set; }
        }

        private static NotificationHubClient _notificationHubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
        (
            AndroidAzureConstants.AndroidAzureConstants.AndroidConsoleFullAccessConnectionString,
            AndroidAzureConstants.AndroidAzureConstants.AndroidConsoleApplicationNotificationHubName,
            true
        );

        [FunctionName("NativeAndroidRegistrationWithTagsAndUsername")]
        public static async Task<HttpResponseMessage> RunNativeAndroidRegistrationWithTagsAndUsername([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetNativeAndroidRegistrationIdPassingHandleUsername/{handleString}")]HttpRequestMessage req, string handleString, TraceWriter log)
        {
            string newRegistrationId = null;

            //GET OBJECT FROM req.Content AS STRING
            var requestContentAsString = await req.Content.ReadAsStringAsync();
            //DESERIALIZE IT
            var deviceRegistrationObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceRegistration>(requestContentAsString);

            //MAKE SURE THERE ARE NO EXISTING REGISTRATIONS FOR THIS PUSH HANDLE(WHICH IS USED BY APPLE AND ANDROID'S SERVICE)
            if (handleString != null)
            {
                var registrations = await _notificationHubClient.GetRegistrationsByChannelAsync(handleString, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await _notificationHubClient.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await _notificationHubClient.CreateRegistrationIdAsync();

            await RegistrationMotion(newRegistrationId, deviceRegistrationObject);

            return req.CreateResponse(System.Net.HttpStatusCode.OK, newRegistrationId);
        }


        public static async Task<HttpResponseMessage> RegistrationMotion(string notificationHubRegistrationId, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = notificationHubRegistrationId;

            //ADD CHECK IF USER IS ALLOWED TO USE THESE TAGS
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            var androidNativeUsername = "AndroidFriendlyUser101";
            registration.Tags.Add("username:" + androidNativeUsername);

            try
            {
                await _notificationHubClient.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            return responseMessage;
        }

        [FunctionName("TemplateAndroidRegistrationWithTagsUsername")]
        public static async Task<HttpResponseMessage> RunTemplateAndroidRegistrationWithTags([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetTemplateAndroidRegistrationWithTagsUsername/{handleString}")]HttpRequestMessage req, string handleString, TraceWriter log)
        {
            string newRegistrationId = null;

            //GET OBJECT FROM req.Content AS STRING
            var requestContentAsString = await req.Content.ReadAsStringAsync();
            //DESERIALIZE IT
            var deviceRegistrationObjectWithTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceRegistrationWithTemplate>(requestContentAsString);

            //if (string.IsNullOrEmpty(deviceRegistrationObjectWithTemplate.name))
            //{ 
            deviceRegistrationObjectWithTemplate.name = "name";
            //}
            deviceRegistrationObjectWithTemplate.expiryTemplate = "0";

            //MAKE SURE THERE ARE NO EXISTING REGISTRATIONS FOR THIS PUSH HANDLE(WHICH IS USED BY APPLE AND ANDROID'S SERVICE)
            if (handleString != null)
            {
                var registrations = await _notificationHubClient.GetRegistrationsByChannelAsync(handleString, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    await _notificationHubClient.DeleteRegistrationAsync(registration);
                }
            }

            ////DIFFERENT FOR APNS and GCM

            //if (newRegistrationId == null)
            //    newRegistrationId = await _notificationHubClient.CreateRegistrationIdAsync();

            var stringTags = deviceRegistrationObjectWithTemplate.Tags;

            Array.Resize(ref stringTags, stringTags.Length + 1);
            var AndroidTemplateUsername = "AndroidTemplateUser101";
            stringTags[stringTags.Length - 1] = "username:" + AndroidTemplateUsername;

            try
            {

                //var regID = _notificationHubClient.CreateGcmTemplateRegistrationAsync(deviceRegistrationObjectWithTemplate.Handle, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, deviceRegistrationObjectWithTemplate.Tags).Result.RegistrationId;
                var regID = _notificationHubClient.CreateGcmTemplateRegistrationAsync(deviceRegistrationObjectWithTemplate.Handle, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, stringTags).Result.RegistrationId;

                Console.WriteLine($"Successful registration of ID: {regID}");

                // await _notificationHubClient.CreateGcmTemplateRegistrationAsync(deviceRegistrationObjectWithTemplate.Handle, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, deviceRegistrationObjectWithTemplate.Tags);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                return responseMessage;
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return responseMessage;
            }
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }

    }
}



//using System.Linq;
//using System.Net;
//using System;
//using System.Linq;
//using System.Diagnostics;
//using System.Collections.Generic;

//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using System.Threading.Tasks;

//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using Microsoft.Azure.WebJobs.Extensions.Http;

//using Microsoft.Azure.NotificationHubs;
//using Microsoft.Azure.NotificationHubs.Messaging;

//namespace AndroidRegistrationFunction
//{
//    public static class NotificationRegistrationViaFunction
//    {

//        //PLEASE AVOID PORT EXHAUSTION: 
//        //https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections

//        public class DeviceRegistration
//        {
//            public string Platform { get; set; }
//            public string Handle { get; set; }
//            public string[] Tags { get; set; }
//        }

//        public class DeviceRegistrationWithTemplate
//        {
//            public string Platform { get; set; }
//            public string Handle { get; set; } //NSData
//            public string[] Tags { get; set; } //NSSet
//            public string jsonBodyTemplates { get; set; }
//            public string expiryTemplate { get; set; }
//            public string name { get; set; }
//        }

//        private static NotificationHubClient _notificationHubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
//        (
//            AndroidAzureConstants.AzureConstants.AndroidConsoleFullAccessConnectionString,
//            AndroidAzureConstants.AzureConstants.AndroidConsoleApplicationNotificationHubName,
//            true
//        );

//        [FunctionName("NativeAndroidRegistrationWithTags")]
//        public static async Task<HttpResponseMessage> RunNativeAndroidRegistrationWithTags([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetNativeAndroidRegistrationIdPassingHandle/{handleString}")]HttpRequestMessage req, string handleString, TraceWriter log)
//        {
//            string newRegistrationId = null;

//            //GET OBJECT FROM req.Content AS STRING
//            var requestContentAsString = await req.Content.ReadAsStringAsync();
//            //DESERIALIZE IT
//            var deviceRegistrationObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceRegistration>(requestContentAsString);

//            //MAKE SURE THERE ARE NO EXISTING REGISTRATIONS FOR THIS PUSH HANDLE(WHICH IS USED BY APPLE AND ANDROID'S SERVICE)
//            if (handleString != null)
//            {
//                var registrations = await _notificationHubClient.GetRegistrationsByChannelAsync(handleString, 100);

//                foreach (RegistrationDescription registration in registrations)
//                {
//                    if (newRegistrationId == null)
//                    {
//                        newRegistrationId = registration.RegistrationId;
//                    }
//                    else
//                    {
//                        await _notificationHubClient.DeleteRegistrationAsync(registration);
//                    }
//                }
//            }

//            if (newRegistrationId == null)
//                newRegistrationId = await _notificationHubClient.CreateRegistrationIdAsync();

//            await RegistrationMotion(newRegistrationId, deviceRegistrationObject);

//            return req.CreateResponse(System.Net.HttpStatusCode.OK, newRegistrationId);
//        }


//        public static async Task<HttpResponseMessage> RegistrationMotion(string notificationHubRegistrationId, DeviceRegistration deviceUpdate)
//        {
//            RegistrationDescription registration = null;
//            switch (deviceUpdate.Platform)
//            {
//                case "mpns":
//                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "wns":
//                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "apns":
//                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "gcm":
//                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                default:
//                    throw new HttpResponseException(HttpStatusCode.BadRequest);
//            }

//            registration.RegistrationId = notificationHubRegistrationId;

//            //ADD CHECK IF USER IS ALLOWED TO USE THESE TAGS
//            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
//            registration.Tags.Add("username:" + "friendlyUser101");

//            try
//            {
//                await _notificationHubClient.CreateOrUpdateRegistrationAsync(registration);
//            }
//            catch (MessagingException e)
//            {
//                ReturnGoneIfHubResponseIsGone(e);
//            }

//            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
//            return responseMessage;
//        }

//        [FunctionName("TemplateAndroidRegistrationWithTags")]
//        public static async Task<HttpResponseMessage> RunTemplateAndroidRegistrationWithTags([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GetTemplateAndroidRegistrationWithTags/{handleString}")]HttpRequestMessage req, string handleString, TraceWriter log)
//        {
//            string newRegistrationId = null;

//            //GET OBJECT FROM req.Content AS STRING
//            var requestContentAsString = await req.Content.ReadAsStringAsync();
//            //DESERIALIZE IT
//            var deviceRegistrationObjectWithTemplate = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceRegistrationWithTemplate>(requestContentAsString);

//            //if (string.IsNullOrEmpty(deviceRegistrationObjectWithTemplate.name))
//            //{ 
//                deviceRegistrationObjectWithTemplate.name = "name";
//            //}
//            deviceRegistrationObjectWithTemplate.expiryTemplate = "0";

//            //MAKE SURE THERE ARE NO EXISTING REGISTRATIONS FOR THIS PUSH HANDLE(WHICH IS USED BY APPLE AND ANDROID'S SERVICE)
//            if (handleString != null)
//            {
//                var registrations = await _notificationHubClient.GetRegistrationsByChannelAsync(handleString, 100);

//                foreach (RegistrationDescription registration in registrations)
//                {
//                    await _notificationHubClient.DeleteRegistrationAsync(registration);
//                }
//            }

//            ////DIFFERENT FOR APNS and GCM

//            //if (newRegistrationId == null)
//            //    newRegistrationId = await _notificationHubClient.CreateRegistrationIdAsync();



//            try
//            {
//                //await _notificationHubClient.CreateGcmTemplateRegistrationAsync(newRegistrationId, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, deviceRegistrationObjectWithTemplate.Tags);
//                var regID = _notificationHubClient.CreateGcmTemplateRegistrationAsync(deviceRegistrationObjectWithTemplate.Handle, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, deviceRegistrationObjectWithTemplate.Tags).Result.RegistrationId;

//                Console.WriteLine($"Successful registration of ID: {regID}");

//               // await _notificationHubClient.CreateGcmTemplateRegistrationAsync(deviceRegistrationObjectWithTemplate.Handle, deviceRegistrationObjectWithTemplate.jsonBodyTemplates, deviceRegistrationObjectWithTemplate.Tags);
//                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
//                return responseMessage;
//            }
//            catch (MessagingException e)
//            {
//                ReturnGoneIfHubResponseIsGone(e);
//                var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
//                return responseMessage;
//            }
//        }

//        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
//        {
//            var webex = e.InnerException as WebException;
//            if (webex.Status == WebExceptionStatus.ProtocolError)
//            {
//                var response = (HttpWebResponse)webex.Response;
//                if (response.StatusCode == HttpStatusCode.Gone)
//                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
//            }
//        }

//    }
//}
