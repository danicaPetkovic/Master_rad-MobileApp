using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace AndrodiFirebaseApp
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT"})]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            //var body = message.GetNotification().Body;
            //Log.Debug(TAG, "Notification Message Body: " + body);
            SendNotification(message.Data);
        }

        void SendNotification(IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(UnknownPersonsActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
                values.Add(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                                                          MainActivity.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            string title, body;
            values.TryGetValue("title", out title);
            values.TryGetValue("body", out body);
            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.notification_icon_background)
                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                                      .SetContentTitle("FCM Message - " + title)
                                      .SetContentText(body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            HttpService.SendRequest();

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
    }
}