using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using heariit_ma.models;
using Android.Provider;

namespace heariit_ma.Fragments
{
    public class ListSongs : Fragment
    {
        List<Datos> items;
        ListView listData;
        AudioAdapter audioAdapter;
        RESTManager manager = new RESTManager();
        SongInfo[] MySongs;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            MySongs = manager.MySongs();
            //if (MySongs.Length == 0)
            //{
            //    Toast.MakeText(this, Application.Resources.GetString(Resource.String.warning_not_songs), ToastLength.Long).Show();
            //}

            View view = inflater.Inflate(Resource.Layout.ListSongs, container, false);

            listData = view.FindViewById<ListView>(Resource.Id.listView1);
            items = new List<Datos>();

            listData.ItemClick += (object sender, AdapterView.ItemClickEventArgs args)
                => listView_ItemClick(sender, args);
            audioCursor();
            return view;

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs E)
        {
            int e = E.Position;
            var item = this.audioAdapter.GetItemAtPosition(e);

            String urlAlbum = item.ArtistAlbum;
            String urlAudio = item.ArrPath;
            String songTitle = item.Title;
            String songArtist = item.Artist;

            //Me verifica si hay un intent actualmente
            //Me verifica si la canción que está sonando ahorita es la misma para no detenerla
            if (e != MediaPlayerRegistry.currentSong)
            {
                if (MediaPlayerRegistry.currentPlayer.IsPlaying) { MediaPlayerRegistry.currentPlayer.Stop(); }
            }

            var intent = new Intent(Activity, typeof(Reproductive));
            intent.PutExtra("urlAlbum", urlAlbum);
            intent.PutExtra("urlAudio", urlAudio);
            intent.PutExtra("songID", e);
            intent.PutExtra("songTitle", songTitle);
            intent.PutExtra("songArtist", songArtist);
            intent.PutExtra("listSize", items.Count);
            this.StartActivity(intent);
        }

        public void setSongs()
        {

            Dictionary<int, String[]> myMusicList = new Dictionary<int, String[]>();

            var length = items.Count;
            for (int i = 0; i < length; i++)
            {
                String[] mu = new String[5];
                var item = this.audioAdapter.GetItemAtPosition(i);
                mu[0] = item.ArtistAlbum;
                mu[1] = item.ArrPath;
                mu[2] = item.Title;
                mu[3] = item.Artist;
                mu[4] = "";
                myMusicList.Add(i, mu);
            }
            MediaPlayerRegistry.Songs = myMusicList;
        }

        private void audioCursor()
        {
            for (int i = 0; i < MySongs.Length; i++)
            {
                SongInfo cs = MySongs[i];
                var audioTitle = cs.title;
                var artist = cs.author;
                //var time = 0;
                //string timestring = convertDuration(Convert.ToInt32(time));
                var arrPath = cs.id.ToString();
                var artistAlbum = "";
                String urlAlbum = "";
                items.Add(new Datos()
                {
                    Title = audioTitle,
                    Artist = artist,
                    ArrPath = arrPath,
                    ArtistAlbum = urlAlbum
                });
            }

            listData.Adapter = audioAdapter = new AudioAdapter(Activity, items);
            setSongs();
        }

        private string convertDuration(long duration)
        {
            long hours = 0;
            string outTime = null;
            try
            {
                hours = (duration / 3600000);
            }
            catch (Exception e)
            {
                return outTime;
            }
            long remaining_minutes = (duration - (hours * 3600000)) / 60000;
            string minutes = Convert.ToString(remaining_minutes);
            if (minutes == "0")
            {
                minutes = "00";
            }

            long remaining_seconds = (duration - (hours * 3600000) - (remaining_minutes * 60000));
            String seconds = Convert.ToString(remaining_seconds);
            if (seconds.Length < 2)
            {
                seconds = "00";
            }
            else
            {
                seconds = seconds.Substring(0, 2);
            }
            if (hours > 0)
            {
                if (minutes.Length < 2)
                {
                    minutes = "0" + minutes;
                }
                outTime = hours + ":" + minutes + ":" + seconds;
            }
            else
            {
                outTime = minutes + ":" + seconds;
            }

            return outTime;
        }

     
    }
}
