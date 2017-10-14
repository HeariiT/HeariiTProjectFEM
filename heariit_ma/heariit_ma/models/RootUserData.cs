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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace heariit_ma.models
{
    public class Data
    {
        public int? id { get; set; }
        public string email { get; set; }
        public string provider { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public string uid { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class RootUserData 
    {
        public Data data { get; set; }
    }
}