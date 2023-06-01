using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using school_trackr_v2.Shared;

namespace school_trackr_v2
{
    [Activity(Label = "ForgotPasswordActivity")]
    public class ForgotPasswordActivity : Activity
    {
        EditText emailAddress;
        TextView valEmailAddress, successSendEmail;
        Button btnSendEmail, btnGoToLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.forgot_password);

            InstantiateWidgets();
            AttachEventHandlers();
        }

        private void InstantiateWidgets()
        {
            emailAddress = FindViewById<EditText>(Resource.Id.emailAddressFP);
            valEmailAddress = FindViewById<TextView>(Resource.Id.valEmailAddressFP);
            successSendEmail = FindViewById<TextView>(Resource.Id.successSendEmail);
            btnSendEmail = FindViewById<Button>(Resource.Id.btnSendEmail);
            btnGoToLogin = FindViewById<Button>(Resource.Id.btnBackToLogin);
        }

        private void AttachEventHandlers()
        {
            btnSendEmail.Click += SendEmail;
            btnGoToLogin.Click += GoToLogin;
        }
        private void SendEmail(object sender, EventArgs e)
        {
            if (!IsValidEmail())
            {
                return;
            }
            //send email stuff
        }

        private void GoToLogin(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(RegisterActivity));
            StartActivity(i);
        }

        private string ResponseIntoString(HttpWebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private void SendEmail(string emailAddress, string password)
        {
            try
            {
                MailMessage clientMail = new MailMessage();
                SmtpClient client = new SmtpClient("smtp.office365.com");
                clientMail.From = new MailAddress(GlobalSettings.EmailAddress, GlobalSettings.SenderName);
                clientMail.To.Add(emailAddress);
                clientMail.Subject = "Forgot Password";
                clientMail.IsBodyHtml = true;
                clientMail.Body = $"Your password is {password}";
                client.EnableSsl = true;
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential(GlobalSettings.EmailAddress, GlobalSettings.Password);
                client.Send(clientMail);
            }
            catch (Exception ex) { }
        }

        public bool IsValidEmail()
        {
            valEmailAddress.Text = "";
            successSendEmail.Text = "";

            if (emailAddress.Text == "")
            {
                valEmailAddress.Text = "Please enter an email address";
                return false;
            }

            string uri = $"find_email_address.php?emailAddress={emailAddress.Text}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = ResponseIntoString(response);

            if (responseString == "Email does not exist")
            {
                valEmailAddress.Text = responseString;
                return false;
            }

            successSendEmail.Text = "Email Sent";
            SendEmail(emailAddress.Text, responseString);
            return true;
        }
    }
}