using Android.App;
using Android.Widget;
using Android.OS;
using heariit_ma.models;
using System.Collections.Generic;
using Android;
using Android.Content.PM;
using Android.Provider;
using Android.Database;
using System;
using Android.Content;
using Android.Media;

namespace heariit_ma
{
    [Activity(Label = "HeariiT", MainLauncher = true)]
    public class MainActivity : Activity{
        private int TRACK_Column, _ID_Column, DATA_Column, YEAR_Column, TITLE_Column;
        private int DURATION_Column, ALBUM_ID_Column, ALBUM_Column, ARTIST_Column;
        List<Datos> items;
        ListView listData;
        AudioAdapter audioAdapter;
        MediaPlayer CurrentPlayer;
        Intent current_intent;
        private int sessionId=0;
        public void SetPlayer(MediaPlayer _player){
            CurrentPlayer = _player;
        }

        public MediaPlayer GetPlayer(){
            return CurrentPlayer;
        }

        protected override void OnCreate(Bundle savedInstanceState){
            current_intent = null;
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            listData = FindViewById<ListView>(Resource.Id.listView1);
            items = new List<Datos>();
            if((int) Build.VERSION.SdkInt >= 23) {
                if((CheckSelfPermission (
                    Manifest.Permission.ReadExternalStorage) != (int) Permission.Granted) 
                || (CheckSelfPermission(
                    Manifest.Permission.WriteExternalStorage) != (int) Permission.Granted)){

                    RequestPermissions(new string[] {
                                            Manifest.Permission.ReadExternalStorage,
                                            Manifest.Permission.ReadExternalStorage
                                        }, 1);
                }
            }
            listData.ItemClick += (object sender, AdapterView.ItemClickEventArgs args)
                => listView_ItemClick(sender, args);
            audioCursor();

            
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e) {
            
            var item = this.audioAdapter.GetItemAtPosition(e.Position);
            String urlAlbum = item.ArtistAlbum;
            String urlAudio = item.ArrPath;
            int position_now = 0;
            if (current_intent != null)
            {
                current_intent = null;

                if (e.Position == MediaPlayerRegistry.currentSong)
                {
                    position_now = MediaPlayerRegistry.currentPlayer.CurrentPosition;
                }
                else
                {
                    if (MediaPlayerRegistry.currentPlayer.IsPlaying) { MediaPlayerRegistry.currentPlayer.Stop(); }
                }

                
            }
            Console.WriteLine(e.Position);
            
            
            var intent = new Intent(this, typeof(Reproductive));
            intent.PutExtra("urlAlbum", urlAlbum);
            intent.PutExtra("urlAudio", urlAudio);
            intent.PutExtra("songID", e.Position);
            current_intent = intent;
            this.StartActivity(current_intent);
            

            

        }

        private void audioCursor(){
            string[] information = {
                MediaStore.Audio.Media.InterfaceConsts.Id,
                MediaStore.Audio.Media.InterfaceConsts.Data,
                MediaStore.Audio.Media.InterfaceConsts.Track,
                MediaStore.Audio.Media.InterfaceConsts.Year,
                MediaStore.Audio.Media.InterfaceConsts.Duration,
                MediaStore.Audio.Media.InterfaceConsts.AlbumId,
                MediaStore.Audio.Media.InterfaceConsts.Album,
                MediaStore.Audio.Media.InterfaceConsts.AlbumKey,
                MediaStore.Audio.Media.InterfaceConsts.Title,
                MediaStore.Audio.Media.InterfaceConsts.TitleKey,
                MediaStore.Audio.Media.InterfaceConsts.ArtistId,
                MediaStore.Audio.Media.InterfaceConsts.Artist
            };
            string orderBy = MediaStore.Audio.Media.InterfaceConsts.Title;
            ICursor audioCursor = ContentResolver.Query(
                MediaStore.Audio.Media.ExternalContentUri, information, null,
                null, orderBy);

            _ID_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id);
            DATA_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Data);
            YEAR_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Year);
            DURATION_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Duration);
            ALBUM_ID_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.AlbumId);
            ALBUM_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Album);
            TRACK_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Track);
            TITLE_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Title);
            ARTIST_Column = audioCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Artist);
            while (audioCursor.MoveToNext()){
                var audioTitle = audioCursor.GetString(TITLE_Column);
                var artist = audioCursor.GetString(ARTIST_Column);
                var time = audioCursor.GetString(DURATION_Column);
                string timestring = convertDuration(Convert.ToInt32(time));
                var arrPath = audioCursor.GetString(DATA_Column);
                var artistAlbum = audioCursor.GetString(ALBUM_ID_Column);
                String urlAlbum = urlAlbumArt(artistAlbum);
                items.Add(new Datos() { Title = audioTitle, Artist = artist, Time=timestring, ArrPath = arrPath,
                                        ArtistAlbum=urlAlbum});
            }

            audioCursor.Close();
            listData.Adapter = audioAdapter = new AudioAdapter(this, items);
        }

        private string convertDuration(long duration) {
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
            if (minutes == "0"){
                minutes = "00";
            }

            long remaining_seconds = (duration - (hours * 3600000) - (remaining_minutes * 60000));
            String seconds = Convert.ToString(remaining_seconds);
            if (seconds.Length < 2){
                seconds = "00";
            }else{
                seconds = seconds.Substring(0, 2);
            }
            if (hours > 0){
                if (minutes.Length < 2){
                    minutes = "0" + minutes;
                }
                outTime = hours + ":" + minutes + ":" + seconds;
            }else{
                outTime = minutes + ":" + seconds;
            }

            return outTime;
        }

        private String urlAlbumArt(String artistAlbum)
        {
            String[] projection = new String[] { MediaStore.Audio.Albums.InterfaceConsts.AlbumArt };
            String selection = MediaStore.Audio.Albums.InterfaceConsts.Id + "=?";
            String [] selectionArgs = new String[] { artistAlbum };
            ICursor cursor = ContentResolver.Query(MediaStore.Audio.Albums.ExternalContentUri, projection, selection, selectionArgs, null);
            String urlAlbum = "";
            if (cursor != null) {
                if (cursor.MoveToFirst()) {
                    urlAlbum = cursor.GetString(cursor.GetColumnIndexOrThrow
                        (MediaStore.Audio.Albums.InterfaceConsts.AlbumArt));
                }
                cursor.Close();
            }
            return urlAlbum;
        }
    }
    

}


