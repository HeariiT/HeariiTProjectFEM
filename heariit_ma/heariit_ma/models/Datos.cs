﻿using System;
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
    public class Datos
    {
        public string Id {get; set;}
        public string Title {get; set;}
        public string Artist {get; set;}
        public string ArrPath {get; set;}
        public string ArtistAlbum {get; set;}
        public string Category { get; set; }
    }
}