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
using Android.Media;

namespace heariit_ma
{
    public class MediaPlayerRegistry{

        public static MediaPlayer currentPlayer = new MediaPlayer();
        public static int currentSong = -1;
        public static Dictionary<int, String[]> Songs;
    }
}