using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using System.Diagnostics;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Web;
using System.Threading.Tasks;

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;

//using PushSampleWebAPIBackend.Models;


namespace NotificationRegistration
{
    public static class NotificationViaFunction
    {
        [FunctionName("NotificationViaFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }


        [FunctionName("GetRegistrationIdPassingHandle")]
        public static async Task<HttpResponseMessage> RunGetRegistrationIdPassingHandle([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "api/GetRegistrationIdPassingHandle/{handleString}")]HttpRequestMessage req, string handleString, TraceWriter log)
        {
            string newRegistrationId = null;

            NotificationHubClient hubClient = Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateClientFromConnectionString
                    (
                        AzureConstants.AzureConstants.ConsoleApplicationFullAccessConnectionString,
                        AzureConstants.AzureConstants.ConsoleApplicationNotificationHubName,
                        true
                    );

            //make sure there are no existing registrations for this push handle(used for ios and android)
            if (handleString != null)
            {
                var registrations = await hubClient.GetRegistrationsByChannelAsync(handleString, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hubClient.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await hubClient.CreateRegistrationIdAsync();

            return req.CreateResponse(System.Net.HttpStatusCode.OK, newRegistrationId);
        }

    public class DeviceRegistration
    {
        public string Platform { get; set; }
        public string Handle { get; set; }
        public string[] Tags { get; set; }
    }
}






            //PUT api/register/5
            //This creates or updates a registration (with provided channeURI) at the specified Id
            public async Task<HttpResponseMessage> Put(string id, DeviceRegistration deviceUpdate)
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

                registration.RegistrationId = id;

                var username = HttpContext.Current.User.Identity.Name;

                //add check if user is allowed to add these tags
                registration.Tags = new HashSet<string>(deviceUpdate.Tags);
                registration.Tags.Add("username:" + username);

                try
                {
                    await hub.CreateOrUpdateRegistrationAsync(registration);
                }
                catch (MessagingException e)
                {
                    ReturnGoneIfHubResponseIsGone(e);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            //DELETE api/register/5
            public async Task<HttpResponseMessage> Delete(string id)
            {
                await hub.DeleteRegistrationAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK);
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

    }
}
