using System;

using Xamarin.Forms;

namespace pushsample
{
    public class App : Application
    {

        public const string NotificationReceivedKey = "NotificationReceived";
        public const string MobileServiceUrl = "http://xamarinpushnotifhubbackend.azurewebsites.net";

        public App()
        {
            // The root page of your application
            //var content = new ContentPage
            //{
            //    Title = "pushsample",
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //            new Label {
            //                HorizontalTextAlignment = TextAlignment.Center,
            //                Text = "Welcome to Xamarin Forms!"
            //            }
            //        }
            //    }
            //};

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
