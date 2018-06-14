using System;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace pushsample
{
    public class PushPage : ContentPage
    {

        Label lblMsg;
        Button btnSend;
        Entry txtMsg;

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
                Children = {messageRecievedLabel, lblMsg}
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

            btnSend = new Button() 
            { 
                WidthRequest = 100,
                Text = "Send",
                BackgroundColor = Color.Blue,
                TextColor = Color.White
            };

            var stackLayoutBottom = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { sendLabel, txtMsg, btnSend}
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children = 
                {
                    stackLayoutTop//,
                    //stackLayoutBottom
                }
            };

        }

		protected override void OnAppearing()
		{
            base.OnAppearing();

            MessagingCenter.Subscribe<object, string>(this, App.NotificationReceivedKey, OnMessageReceived);
            btnSend.Clicked += OnBtnSendClicked;
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

        async void OnBtnSendClicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"Sending message: " + txtMsg.Text);

            //This will be for the sending and triggering of notifications
            //var content = new StringContent("\"" + txtMsg.Text + "\"", Encoding.UTF8, "application/json");
            //var result = await _client.PostAsync("xamunotifications", content);
            //Debug.WriteLine("Send result: " + result.IsSuccessStatusCode);
        }




	}



}

