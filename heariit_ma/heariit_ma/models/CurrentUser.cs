using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace heariit_ma.models
{
    class CurrentUser
    {
        public static int id { get; set; }
        public static string email { get; set; }
        public static string first_name { get; set; }
        public static string last_name { get; set; }
        public static string username { get; set; }

        public static string x_access_token { set; get; }

        public CurrentUser() { }
    }
}