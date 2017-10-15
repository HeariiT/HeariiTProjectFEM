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
using Newtonsoft.Json;
using Android.Preferences;

namespace heariit_ma
{
    [Activity(Label = "HeariiT - ", Icon = "@drawable/icon")]
    public class MainActivity : Activity {
        List<Datos> items;
        ListView listData;
        AudioAdapter audioAdapter;
        RESTManager manager = new RESTManager();
        SongInfo[] MySongs;

        protected override void OnCreate(Bundle savedInstanceState) {
            
            base.OnCreate(savedInstanceState);

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if ((CheckSelfPermission(
                    Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
                || (CheckSelfPermission(
                    Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted))
                {

                    RequestPermissions(new string[] {
                                            Manifest.Permission.ReadExternalStorage,
                                            Manifest.Permission.ReadExternalStorage
                                        }, 1);
                }
            }

            CurrentUser.x_access_token = Intent.GetStringExtra("x-access-token");
            /**
             * Guardar el token en el almacenamiento del telefono, para no tener que hacer
             * login de nuevo cuando se cierra la app
            **/
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("x-access-token", CurrentUser.x_access_token).Apply();

            Console.WriteLine("El token es: " + CurrentUser.x_access_token);
            Console.WriteLine("El username es: " + CurrentUser.username);
            this.Window.SetTitle("Welcome, " + CurrentUser.username);

            MySongs = manager.MySongs();
            if (MySongs.Length == 0)
            {
                Toast.MakeText(this, Application.Resources.GetString(Resource.String.warning_not_songs), ToastLength.Long).Show();
            }
            SetContentView(Resource.Layout.Main);
            Button UploadBtn = FindViewById<Button>(Resource.Id.mainUploadBtn);
            Button SignOutBtn = FindViewById<Button>(Resource.Id.mainSignOutBtn);

            listData = FindViewById<ListView>(Resource.Id.fileManagerList);
            items = new List<Datos>();

            listData.ItemClick += (object sender, AdapterView.ItemClickEventArgs args)
                => listView_ItemClick(sender, args);
            audioCursor();

            listData.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs args)
                    => listView_ItemLongClick(sender, args);

            UploadBtn.Click += delegate
            {
                var UploadActivity = new Intent(this, typeof(Uploader));
                UploadActivity.PutExtra("x-access-token", CurrentUser.x_access_token);
                
                this.StartActivity(UploadActivity);
                this.Finish();
                
            };

            SignOutBtn.Click += delegate
            {
                if( manager.SignOut( ) )
                {
                    var LoginActivity = new Intent(this, typeof(LoginScreen));
                    if (MediaPlayerRegistry.currentPlayer.IsPlaying) { MediaPlayerRegistry.currentPlayer.Stop(); }                    
                    this.StartActivity(LoginActivity);
                    this.Finish();
                }                
            };
            
        }
        
        void updateList()
        {
            listData = FindViewById<ListView>(Resource.Id.fileManagerList);
            items = new List<Datos>();
            audioCursor();
        }

        void listView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs E){
            int e = E.Position;
            var item = this.audioAdapter.GetItemAtPosition(e);
            int id = MySongs[e].id;
            string category_name = item.Category;
            var intent = new Intent(this, typeof(CategoryChooser));
            intent.PutExtra("currentCategory", category_name);
            intent.PutExtra("songId", id);
            this.StartActivity(intent);
            this.Finish();
        }
        
        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs E) {
            int e = E.Position;
            var item = this.audioAdapter.GetItemAtPosition(e);

            String urlAlbum = item.ArtistAlbum;
            String urlAudio = item.ArrPath;
            String songTitle = item.Title;
            String songArtist = item.Artist;

            //Me verifica si hay un intent actualmente
                //Me verifica si la canción que está sonando ahorita es la misma para no detenerla
            if (e != MediaPlayerRegistry.currentSong) {
                if (MediaPlayerRegistry.currentPlayer.IsPlaying) { MediaPlayerRegistry.currentPlayer.Stop();}
            }
            
            var intent = new Intent(this, typeof(Reproductive));
            intent.PutExtra("urlAlbum", urlAlbum);
            intent.PutExtra("urlAudio", urlAudio);
            intent.PutExtra("songID", e);
            intent.PutExtra("songTitle", songTitle);
            intent.PutExtra("songArtist", songArtist);
            intent.PutExtra("listSize", items.Count);
            this.StartActivity(intent);
        }

        public void setSongs(){

            Dictionary<int, String[]> myMusicList = new Dictionary<int, String[]>();
            
            var length = items.Count;
            for (int i = 0; i < length; i++){
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

        private void audioCursor(){
            for (int i = 0; i < MySongs.Length; i++){
                SongInfo cs = MySongs[i];
                var audioTitle = cs.title;
                var artist = cs.author;
                //var time = 0;
                //string timestring = convertDuration(Convert.ToInt32(time));
                var arrPath = cs.id.ToString();
                var artistAlbum = "";
                String urlAlbum = urlAlbumArt(artistAlbum);
                var category = manager.getSongCategory(arrPath);
                items.Add(new Datos() { Title = audioTitle, Artist = artist, ArrPath = arrPath,
                    ArtistAlbum = urlAlbum, Category = category });
            }
            
            listData.Adapter = audioAdapter = new AudioAdapter(this, items);
            setSongs();
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

        private String urlAlbumArt(String artistAlbum){
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


