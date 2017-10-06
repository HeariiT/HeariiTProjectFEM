using Android.App;
using Android.Widget;
using Android.OS;
using heariit_ma.models;
using System.Collections.Generic;

namespace heariit_ma
{
    [Activity(Label = "heariit_ma", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private int TRACK_Column, _ID_Column, DATA_Column, YEAR_Column;
        private int DURATION_Column, ALBUM_ID_Column, ALBUM_Column, ARTIST_Column;
        List<Datos> items;
        ListView listData;
        AudioAdapter audioAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            listData = FindViewById<ListView>(Resource.Id.listView1);
            items = new List<Datos>();

        }
    }
}

