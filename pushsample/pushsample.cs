using System;

using Xamarin.Forms;

namespace pushsample
{
    public class App : Application
    {

        public const string NotificationReceivedKey = "NotificationReceived";
        //public const string MobileServiceUrl = "http://xamarinpushnotifhubbackend.azurewebsites.net";

        //DELETE
        //// Azure app-specific connection string and hub path
        ////    "ConnectionString - Microsoft documentation recommends to use the Listen on client // Look under Access Policies";
        //public const string ConnectionString = "Endpoint=sb://samplenotifhubtwonamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=1+Y3vZK+Im9iZFuuSNXR3hP06I4vLJipqdtCsu1A75Y=";
        ////"<Azure hub name - ie. just the name of the Notificatin Hub>"; 
        //public const string NotificationHubName = "MySampleNotificationHub";

        public App()
        {

            var pushPage = new PushPage();
            MainPage = new NavigationPage(pushPage);

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
