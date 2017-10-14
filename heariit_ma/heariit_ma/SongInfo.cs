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

namespace heariit_ma
{
    public class SongInfo
    {
        public string _id { get; set; }
        public int id { get; set; }
        public int user { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string album { get; set; }
        public int __v { get; set; }
    }
}