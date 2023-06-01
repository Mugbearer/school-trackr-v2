using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace school_trackr_v2.Shared
{
    internal class GlobalSettings
    {
        public static string URI { get; } = "http://192.168.68.111/IT140P/school-trackr-rest/"; //Change this according to your IP address and directory

        public static string EmailAddress { get; } = "kris.p.bacon2023@outlook.com";

        public static string Password { get; } = "SamplePassword123";

        public static string SenderName { get; } = "Kris P. Bacon";
    }
}