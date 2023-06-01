using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using school_trackr_v2.Shared;

namespace school_trackr_v2
{
    [Activity(Label = "School Trackr", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText username, password;
        TextView valUsername, valPassword, btnGoToRegister, btnGoToForgotPassword;
        ImageButton btnLogIn;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.login);

            InstantiateWidgets();
            AttachEventHandlers();
        }

        private void InstantiateWidgets()
        {
            username = FindViewById<EditText>(Resource.Id.usernameL);
            password = FindViewById<EditText>(Resource.Id.passwordL);
            valUsername = FindViewById<TextView>(Resource.Id.valUsernameL);
            valPassword = FindViewById<TextView>(Resource.Id.valPasswordL);
            btnLogIn = FindViewById<ImageButton>(Resource.Id.btnLogIn);
            btnGoToRegister = FindViewById<TextView>(Resource.Id.btnGoToRegister);
            btnGoToForgotPassword = FindViewById<TextView>(Resource.Id.btnGoToForgotPassword);
        }

        private void AttachEventHandlers()
        {
            btnLogIn.Click += LogIn;
            btnGoToRegister.Click += GoToRegister;
            btnGoToForgotPassword.Click += GoToForgotPassword;
        }

        private void LogIn(object sender, EventArgs e)
        {
            if (!IsValidLogin())
            {
                return;
            }
            Intent i = new Intent(this, typeof(CRUDActivity));
            StartActivity(i);
        }

        private void GoToRegister(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(RegisterActivity));
            StartActivity(i);
        }

        private void GoToForgotPassword(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(ForgotPasswordActivity));
            StartActivity(i);
        }

        private string ResponseIntoString(HttpWebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private bool IsValidLogin()
        {
            valUsername.Text = "";
            valPassword.Text = "";

            if (username.Text == "")
            {
                valUsername.Text = "Please Enter Username";
                return false;
            }
            if (password.Text == "")
            {
                valPassword.Text = "Please Enter Password";
                return false;
            }

            string uri = $"log_in.php?username={username.Text}&password={password.Text}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = ResponseIntoString(response);

            if (responseString == "Username Does Not Exist")
            {
                valUsername.Text = responseString;
                return false;
            }
            else if (responseString == "Invalid Password")
            {
                valPassword.Text = responseString;
                return false;
            }

            Toast.MakeText(this, responseString, ToastLength.Long).Show();
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}