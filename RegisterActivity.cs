using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using school_trackr_v2.Shared;

namespace school_trackr_v2
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        EditText emailAddress, username, password;
        TextView valEmailAddress, valUsername, valPassword, successRegister;
        Button btnRegister, btnGoToLogin;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);

            InstantiateWidgets();
            AttachEventHandlers();
        }

        private void InstantiateWidgets()
        {
            emailAddress = FindViewById<EditText>(Resource.Id.emailAddressR);
            username = FindViewById<EditText>(Resource.Id.usernameR);
            password = FindViewById<EditText>(Resource.Id.passwordR);
            valEmailAddress = FindViewById<TextView>(Resource.Id.valEmailAddressR);
            valUsername = FindViewById<TextView>(Resource.Id.valUsernameR);
            valPassword = FindViewById<TextView>(Resource.Id.valPasswordR);
            successRegister = FindViewById<TextView>(Resource.Id.successRegister);
            btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            btnGoToLogin = FindViewById<Button>(Resource.Id.btnBackToLogin);
        }

        private void AttachEventHandlers()
        {
            btnRegister.Click += Register;
            btnGoToLogin.Click += GoToLogin;
        }
        private void Register(object sender, EventArgs e)
        {
            if (!IsValidRegister())
            {
                return;
            }
        }

        private void GoToLogin(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }

        private string ResponseIntoString(HttpWebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private bool IsValidRegister()
        {
            valEmailAddress.Text = "";
            valUsername.Text = "";
            valPassword.Text = "";

            if (emailAddress.Text == "" || username.Text == "" || password.Text == "" || !emailAddress.Text.Contains("@") || !emailAddress.Text.Contains(".com") || password.Text.Length < 8)
            {
                if (emailAddress.Text == "")
                {
                    valEmailAddress.Text = "Please enter an email address";
                }
                if (username.Text == "")
                {
                    valUsername.Text = "Please enter username";
                }
                if (password.Text == "")
                {
                    valPassword.Text = "Please enter password";
                }
                if (!emailAddress.Text.Contains("@") || !emailAddress.Text.Contains(".com"))
                {
                    valEmailAddress.Text = "Please enter a valid email address";
                }
                if (password.Text.Length < 8)
                {
                    valPassword.Text = "Password must be at least 8 characters";
                }
                return false;
            }

            string uri = $"register.php?emailAddress={emailAddress.Text}&username={username.Text}&password={password.Text}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = ResponseIntoString(response);


            if (responseString.Contains("Email Error") || responseString.Contains("Username Error"))
            {
                if (responseString.Contains("Email Error"))
                {
                    valEmailAddress.Text = "Email is already taken";
                }
                if (responseString.Contains("Username Error"))
                {
                    valEmailAddress.Text = "Username is already taken";
                }
                return false;
            }

            successRegister.Text = responseString;
            return true;
        }
    }
}