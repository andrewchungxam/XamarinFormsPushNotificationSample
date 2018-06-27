using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using AndroidAzureConstants;
using Xamarin.Forms;

namespace pushsample
{
    public class PushPage : ContentPage
    {

        Label lblMsg;
        Button btnSend_Native;
        Button btnSend_Template;
        Button btnSend_MultipleTemplate;

        Entry txtMsg;

        private static HttpClient _httpClient = new HttpClient() { };

        public PushPage()
        {

            //TOP STACKLAYOUT
            var messageRecievedLabel = new Label()
            {
                Text = "Received message:"
            };

            lblMsg = new Label()
            {
                Text = "Nothing yet",
                TextColor = Color.Gray
            };

            var stackLayoutTop = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { messageRecievedLabel, lblMsg }
            };

            //BOTTOM STACKLAYOUT
            var sendLabel = new Label()
            {
                Text = "Send:"
            };

            txtMsg = new Entry()
            {
                WidthRequest = 150
            };

            btnSend_Native = new Button()
            {
                WidthRequest = 100,
                Text = "Native",
                BackgroundColor = Color.Blue,
                TextColor = Color.White
            };


            btnSend_Template = new Button()
            {
                WidthRequest = 100,
                Text = "Template",
                BackgroundColor = Color.Blue,
                TextColor = Color.White
            };


            btnSend_MultipleTemplate = new Button()
            {
                WidthRequest = 100,
                Text = "MultiTemplate",
                BackgroundColor = Color.Blue,
                TextColor = Color.White
            };

            var stackLayoutBottom = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //Children = { sendLabel, txtMsg, btnSend}
                Children = { sendLabel, btnSend_Native, btnSend_Template, btnSend_MultipleTemplate }
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    stackLayoutTop,
                    stackLayoutBottom
                }
            };

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object, string>(this, App.NotificationReceivedKey, OnMessageReceived);
            btnSend_Native.Clicked += OnBtnSendClicked_Native;
            btnSend_Template.Clicked += OnBtnSendClicked_Template;
            btnSend_MultipleTemplate.Clicked += OnBtnSendClicked_MultipleTemplate;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<object>(this, App.NotificationReceivedKey);
        }

        void OnMessageReceived(object sender, string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lblMsg.Text = msg;
            });
        }


        async void OnBtnSendClicked_Native(object sender, EventArgs e)
        {
            Debug.WriteLine($"Sending message notification MultipleTemplate to URI {AzureNotificationsViaFunctionsURLS.AndroidNativeApiURL}");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(AzureNotificationsViaFunctionsURLS.AndroidNativeApiURL),
            };

            var result = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

            Debug.WriteLine("Send result: " + result.IsSuccessStatusCode);
        }


        async void OnBtnSendClicked_Template(object sender, EventArgs e)
        {
            Debug.WriteLine($"Sending message notification MultipleTemplate to URI {AzureNotificationsViaFunctionsURLS.AndroidTemplateApiURL}");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(AzureNotificationsViaFunctionsURLS.AndroidTemplateApiURL),
            };

            var result = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

            Debug.WriteLine("Send result: " + result.IsSuccessStatusCode);
        }

        async void OnBtnSendClicked_MultipleTemplate(object sender, EventArgs e)
        {
            Debug.WriteLine($"Sending message notification MultipleTemplate to URI {AzureNotificationsViaFunctionsURLS.AndroidMultipleTemplateApiURL}");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(AzureNotificationsViaFunctionsURLS.AndroidMultipleTemplateApiURL),
            };

            var result = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);

            Debug.WriteLine("Send result: " + result.IsSuccessStatusCode);
        }


    }
}

