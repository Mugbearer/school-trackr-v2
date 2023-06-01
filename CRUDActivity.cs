using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Win32;
using school_trackr_v2.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using static Android.Provider.DocumentsContract;
using static Java.Util.Jar.Attributes;

namespace school_trackr_v2
{
    [Activity(Label = "CRUDActivity")]
    public class CRUDActivity : Activity
    {
        EditText fullName, school;
        TextView valFullName, valSchool, valCountry, successCRUD;
        RadioGroup gender;
        RadioButton male, female, unspecified;
        AutoCompleteTextView country;
        Button btnAdd, btnSearch, btnUpdate, btnDelete, btnLogOut;
        string responseString, genderValue = "unspecified", required = "Required Field";
        HttpWebRequest request;
        HttpWebResponse response;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.crud_students);

            InstantiateWidgets();
            AttachEventHandlers();
            SetCountries();
        }
        private void InstantiateWidgets()
        {
            fullName = FindViewById<EditText>(Resource.Id.fullName);
            school = FindViewById<EditText>(Resource.Id.school);
            valFullName = FindViewById<TextView>(Resource.Id.valFullName);
            valSchool = FindViewById<TextView>(Resource.Id.valSchool);
            valCountry = FindViewById<TextView>(Resource.Id.valCountry);
            successCRUD = FindViewById<TextView>(Resource.Id.successCRUD);
            gender = FindViewById<RadioGroup>(Resource.Id.gender);
            male = FindViewById<RadioButton>(Resource.Id.male);
            female = FindViewById<RadioButton>(Resource.Id.female);
            unspecified = FindViewById<RadioButton>(Resource.Id.unspecified);
            country = FindViewById<AutoCompleteTextView>(Resource.Id.country);
            btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
            btnSearch = FindViewById<Button>(Resource.Id.btnSearch);
            btnUpdate = FindViewById<Button>(Resource.Id.btnUpdate);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            btnLogOut = FindViewById<Button>(Resource.Id.btnLogOut);
        }

        private void AttachEventHandlers()
        {
            btnAdd.Click += Add;
            btnSearch.Click += Search;
            btnUpdate.Click += Update;
            btnDelete.Click += Delete;
            btnLogOut.Click += LogOut;
            gender.CheckedChange += Gender_CheckedChange;
        }

        private void SetCountries()
        {
            string[] countries = { "Cambodia", "Indonesia", "Philippines", "Thailand", "Singapore" };
            ArrayAdapter adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, countries);
            country.Adapter = adapter;
        }

        private void Gender_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int checkedId = gender.CheckedRadioButtonId;
            RadioButton checkedGender = FindViewById<RadioButton>(checkedId);
            if (checkedGender.Text == "I don't want to specify")
            {
                genderValue = "Unspecified";
            }
            else
            {
                genderValue = checkedGender.Text;
            }
        }

        private void Add(object sender, EventArgs e)
        {
            if (!IsValidInputs())
            {
                return;
            }

            string uri = $"add_student.php?fullName={fullName.Text}&gender={genderValue}&school={school.Text}&country={country.Text}";
            request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            response = (HttpWebResponse)request.GetResponse();
            responseString = ResponseIntoString(response);

            if (responseString == "Student Already Exists")
            {
                valFullName.Text = responseString;
                successCRUD.Text = "";
                return;
            }
            valFullName.Text = "";
            successCRUD.Text = responseString;
        }

        private void Search(object sender, EventArgs e)
        {
            if (!IsValidFullName())
            {
                return;
            }

            string uri = $"search_student.php?fullName={fullName.Text}";
            request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            responseString = ResponseIntoString(response);

            if (responseString == "Student Not Found")
            {
                valFullName.Text = responseString;
                return;
            }

            request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            response = (HttpWebResponse)request.GetResponse();
            Dictionary<string, string> studentData = ResponseIntoDictionary(response);

            school.Text = studentData["school"];
            country.Text = studentData["country"];
            switch (studentData["gender"])
            {
                case "Male":
                    male.Checked = true;
                    break;
                case "Female":
                    female.Checked = true;
                    break;
                case "Unspecified":
                    unspecified.Checked = true;
                    break;
            }
        }

        private void Update(object sender, EventArgs e)
        {
            if (!IsValidInputs())
            {
                return;
            }

            string uri = $"update_student.php?fullName={fullName.Text}&gender={genderValue}&school={school.Text}&country={country.Text}";
            request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            response = (HttpWebResponse)request.GetResponse();
            responseString = ResponseIntoString(response);

            if (responseString == "Student Does Not Exist")
            {
                valFullName.Text = responseString;
                successCRUD.Text = "";
                return;
            }
            valFullName.Text = "";
            successCRUD.Text = responseString;
        }

        private void Delete(object sender, EventArgs e)
        {
            if (!IsValidFullName())
            {
                return;
            }

            string uri = $"delete_student.php?fullName={fullName.Text}";
            request = (HttpWebRequest)WebRequest.Create(GlobalSettings.URI + uri);
            response = (HttpWebResponse)request.GetResponse();
            responseString = ResponseIntoString(response);

            if (responseString == "Student Does Not Exist")
            {
                valFullName.Text = responseString;
                successCRUD.Text = "";
                return;
            }
            valFullName.Text = "";
            successCRUD.Text = responseString;
        }

        private void LogOut(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(MainActivity));
            StartActivity(i);
        }

        private string ResponseIntoString(HttpWebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        private Dictionary<string, string> ResponseIntoDictionary(HttpWebResponse response)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            var result = reader.ReadToEnd();
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;

            Dictionary<string, string> studentData = new Dictionary<string, string>();
            studentData["gender"] = root.GetProperty("gender").ToString();
            studentData["school"] = root.GetProperty("school").ToString();
            studentData["country"] = root.GetProperty("country").ToString();

            return studentData;
        }

        private bool IsValidFullName()
        {
            ClearValidationMessages();

            if (fullName.Text == "")
            {
                valFullName.Text = required;
                return false;
            }

            return true;
        }

        private bool IsValidInputs()
        {
            ClearValidationMessages();

            bool isValid = true;

            if (fullName.Text == "")
            {
                valFullName.Text = required;
                isValid = false;
            }
            if (school.Text == "")
            {
                valSchool.Text = required;
                isValid = false;
            }
            if (country.Text == "")
            {
                valCountry.Text = required;
                isValid = false;
            }

            return isValid;
        }

        private void ClearValidationMessages()
        {
            valFullName.Text = "";
            valSchool.Text = "";
            valCountry.Text = "";
        }
    }
}