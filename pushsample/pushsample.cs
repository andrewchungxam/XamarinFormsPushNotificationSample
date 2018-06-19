using System;

using Xamarin.Forms;

namespace pushsample
{
    public class App : Application
    {
        
        // Azure app-specific connection string and hub path
        //    "ConnectionString - Microsoft documentation recommends to use the Listen on client ";
        //public const string ConnectionString = ;
        //////"<Azure hub name - ie. just the name of the hub>"; 
        //public const string NotificationHubName = ;

        public const string NotificationReceivedKey = "NotificationReceived";
        //public const string MobileServiceUrl = "http://xamarinpushnotifhubbackend.azurewebsites.net";


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
