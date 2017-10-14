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
    public class Errors
    {        
        public List<string> full_messages { get; set; }
    }

    public class RootSignUpData
    {
        public string status { get; set; }
        public Data data { get; set; }
        public Errors errors { get; set; }
    }
}