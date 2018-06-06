using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using PushSampleWebAPIBackend.Models;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;

namespace PushSampleWebAPIBackend.Controllers
{
    public class NotificationsController : ApiController
    {
        //
        //ASSUMPTION OF NATIVE REGISTRATION 
        //

        public async Task<HttpResponseMessage> Post(string pns, [FromBody]string message, string to_tag)
        {
            var user = HttpContext.Current.User.Identity.Name;
            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;
            userTag[1] = "from:" + user;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            switch (pns.ToLower())
            {
                case "wns":
                    //Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                        "From " + user + ": " + message + "</text></binding></visual></toast>";
                    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    //IOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "gcm":
                    var notif = "{\"data\" : {\"message\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                    break;
            }

            if (outcome != null)
            {
                if (!(
                        (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                        (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)
                    ))
                {
                    ret = HttpStatusCode.OK;
                }
            }

            return Request.CreateResponse(ret);
        }
    }    
}
