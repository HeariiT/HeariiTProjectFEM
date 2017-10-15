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
    public class CategoryData
    {
        public string category_name { get; set; }
        public string category_id { get; set; }
        public string error { get; set; }
    }
}