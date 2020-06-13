using System;
using System.IO;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;

namespace AndrodiFirebaseApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        internal int imageCount = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var token = FirebaseInstanceId.Instance.Token;
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            CreateNotificationChannel();

            //HttpService.SendRequest();
            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            var i = 1;
            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            foreach (string file in Directory.EnumerateFiles(filePath))
            {
                var contents = File.ReadAllBytes(file);

                Bitmap imageBitmap = BitmapFactory.DecodeByteArray(contents, 0, contents.Length);

                ImageView imageView = new ImageView(this.ApplicationContext)
                {
                    Id = i + 100,
                    LayoutParameters = new ViewGroup.LayoutParams(300, 300),
                    ContentDescription = "Uknown person",
                };
                imageView.SetImageBitmap(imageBitmap);

                EditText editText = new EditText(this.ApplicationContext)
                {
                    Id = i,
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                    ContentDescription = "Uknown person name",
                    Text = "Set person's name.."
                };

                LinearLayout linearLayout = new LinearLayout(this.ApplicationContext)
                {
                    Orientation = Orientation.Horizontal,
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                };

                linearLayout.AddView(imageView);
                linearLayout.AddView(editText);
                layout.AddView(linearLayout);
                i++;
            }
            imageCount = i - 1;
            TextView text = FindViewById<TextView>(Resource.Id.labelNothingDetected);
            text.Text = imageCount > 0 ? "" : "No unknown persons detected!";
    }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            string response = "";
            for(var i = 1; i < imageCount +1; i++)
            {
                TextView text = FindViewById<TextView>(i);
                response += text.Text + ",";
            }
            response = response.Remove(response.Length - 1);
            HttpService.SendResponse(response);
            Toast.MakeText(this.ApplicationContext, "Information is send!", ToastLength.Long);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, "FCM Notifications", NotificationImportance.Default)
            {
                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            foreach (string file in Directory.EnumerateFiles(filePath))
            {
                File.Delete(file);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";
            foreach (string file in Directory.EnumerateFiles(filePath))
            {
                File.Delete(file);
            }
        }
    }
}