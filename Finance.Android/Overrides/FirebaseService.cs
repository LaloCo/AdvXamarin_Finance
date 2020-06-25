using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Finance.View;
using Firebase.Messaging;
using WindowsAzure.Messaging;

namespace Finance.Droid.Overrides
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT"})]
    public class FirebaseService : FirebaseMessagingService
    {
        public FirebaseService()
        {
        }

        public override void OnNewToken(string p0)
        {
            SendRegistrationToAzure(p0);
        }

        void SendRegistrationToAzure(string token)
        {
            try
            {
                NotificationHub hub = new NotificationHub("FinanceNotificationHub", "Endpoint=sb://financenh.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=wfcFEg/P2SGcVOHINW8nz9k5gIaZ4gnDUJqGUbWWv/g=", this);

                // register device with Notification Hub using FCM token
                Registration registration = hub.Register(token, new string[] { "default" });

                // subscribe to the tags with template
                string pnsHandle = registration.PNSHandle;
                TemplateRegistration templateRegistration = hub.RegisterTemplate(pnsHandle, "defaultTemplate", "{\"data\":{\"message\":\"$(messageParam)\"}}", new string[] { "default" });
            }
            catch(Exception ex)
            {

            }
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            string messageBody = string.Empty;

            if (message.GetNotification() != null)
            {
                messageBody = message.GetNotification().Body;
            }

            // NOTE: test messages sent via the Azure portal will be received here
            else
            {
                messageBody = message.Data.Values.First();
            }

            // convert the incoming message to a local notification
            SendLocalNotification(messageBody);

            // could send to Xamarin.Forms app
            // perhaps creating an AddMessage method back in the MainPage
            // and calling it like this:
            // (App.Current.MainPage as MainPage)?.AddMessage(messageBody);
        }

        void SendLocalNotification(string body)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra("message", body);

            //Unique request code to avoid PendingIntent collision.
            var requestCode = new Random().Next();
            var pendingIntent = PendingIntent.GetActivity(this, requestCode, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this)
                .SetContentTitle("Finance Message")
                .SetSmallIcon(Resource.Drawable.ic_launcher)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent);


            var notificationManager = NotificationManager.FromContext(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel(
                                        "XamarinNotifyChannel",
                                        "Finance App",
                                        NotificationImportance.High);
                notificationManager.CreateNotificationChannel(channel);

                notificationBuilder.SetChannelId("XamarinNotifyChannel");
            }

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
