# XamarinFormsPushNotificationSample
This sample is a progression based on the Git branches.  You can switch Branches using the GitHub desktop client or Command line. <br ><br >
You should looking at the code starting from Branch 01<br ><br >
You can find the PDFs in the Master Branch.<br ><br >
<br ><br >
This is a sample that simplifies the usage of Azure Notifications and Xamarin.Forms.  <br ><br >
There are PDFs that will help with gettings started.  Triggering the Notification will be through the Azure portal + a program called "Pusher" + and later on a Console application + trigger it via Azure Functions.  Note when you use Azure Functions - you can either run the program locally from Visual Studio and then trigger the function via Postman (pointing at the local endpoint) or publish the application and then via Postman (pointing at the Azure endpoint).<br ><br >
Notes:<br ><br >
- Push notifications will only work on real devices; you can run the app on simulator but it will not receive notifications<br >
- Real devices require special provisioning profiles from Apple to work; the PDFs and links contained therein will guide you <br >
- For your production environment (iOS) you will be required to spin up a separate notification hub
- There is a difference between Native registration and Template registration -- the difference may be subtle in the code but at the time of "triggering the notification" -- you will need to send a Native Notification to a Native Registration or send a Template Notification to a Template Registration.  This will become more relevant in the later branches.  
- Note: you'll probably want to send notifications from the Azure portal (test send) -- this will work for Native Registrations and will start working in Branch 2.  Even after Branch 2 - you'll only be able to use the portal to send Native Notifications; Template Notifications will be sent via the tools you'll create on Branch 3 and on
- If you have issues building when you switch branches - try do a BUILD > CLEAN ALL in Visual Studio and retrying the build
- If you want to re-trigger a registration - you have to DELETE your app from your phone first.  A rebuild alone is not sufficient<br ><br >

iOS only:
Branches were made and kept for checkpoints in project:<br ><br >
Branch 1 - Xamarin Forms apps (iOS only + receiving push notifications from "Pusher app") <br ><br >
Branch 2 - Branch 1 + registered with Azure Notification Hub directly + receiving test Push Notications from the Azure portal) <br ><br >
Branch 3 - Branch 2 + Ability to create specialized Notification Tags ("Similar to topic subscriptions") + Console Application to trigger Notifications <br ><br >
Branch 4 - Branch 3 + Xamarin Forms app (receiving push notifications + Triggering notifications via Functions + Postman (Or Console from previous project) <br ><br >
Branch 5 - Branch 4 + Use Functions to register devices <br ><br >
Branch 6 - Branch 5 + Use Functions to registering a username tag + trigger a notification to Usernme via Function or Console application ) <br ><br >
Branch 7 - Branch 6 + Use buttons on Forms app to trigger notification <br ><br >
ANDROID: <br ><br >
Branch 8 - Branch 7 + Android + Native Notifications + Azure Notification Hub Test Send<br ><br > 
Branch 9 - Branch 8 + Android + Template Notifications + Console Send<br ><br > 
Branch 10 - Branch 9 + Android + Triggering notifications via Functions + Postman (Or Console from previous project)<br ><br > 
Branch 11 - Branch 10 + Android + Use Functions to register devices <br ><br >
Branch 12 - Branch 11 + Android + Use Functions to registering a username tag + trigger a notification to Usernme via Function or Console application )<br ><br >
Branch 14 - Branch 12 + Android + Use buttons on Forms app to trigger notification<br ><br >  
<br >
<br >
This Push notification sample is inspired by and based on projects by my colleagues: <br >
Rob DeRosa https://github.com/rob-derosa/Hunt <br >
Mahdi https://github.com/malirezai/Xamarin-Forms-Push-Notifications-Sample <br >
Rene https://github.com/Krumelur/XamUAzureNotificationHub and video https://docs.microsoft.com/en-us/xamarin/xamarin-forms/data-cloud/push-notifications/azure <br >
and this Microsoft Document: https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-ios-push-notification-apns-get-started <br >


<br >
<br >

MIT License

Copyright (c) 2018 andrewchungxam

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
