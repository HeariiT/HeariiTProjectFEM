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
using Android.Media;
using static Android.Views.View;
using Java.IO;
using Android.Graphics;


namespace heariit_ma
{
    [Activity(Label = "HeariiT Music")]
    public class Reproductive : Activity, MediaController.IMediaPlayerControl, IOnTouchListener, IOnClickListener {

        MediaPlayer _player;
        MediaController mediaController;
        LinearLayout linearLayout;
        ImageView imgAlbum;
        TextView titleSong;
        TextView artistSong;

        public int AudioSessionId{get { return 0;}}
        public int BufferPercentage { get { return _player.CurrentPosition *100 /_player.Duration; } }
        public int CurrentPosition {get { return _player.CurrentPosition; } }
        public int Duration {get { return _player.Duration; } }
        public bool IsPlaying {get { return _player.IsPlaying; } }

       


        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Reproductive);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            imgAlbum = FindViewById<ImageView>(Resource.Id.imageView_Album);
            linearLayout.SetOnTouchListener(this);
            String urlAlbum = this.Intent.GetStringExtra("urlAlbum");
            String urlAudio = this.Intent.GetStringExtra("urlAudio");
            _player = new MediaPlayer();
            mediaController = new Android.Widget.MediaController(this, true);

            //Se Asignan los Titulos y Artista
            titleSong = FindViewById<TextView>(Resource.Id.reproductive_title);
            artistSong = FindViewById<TextView>(Resource.Id.reproductive_artist);
            int songID = this.Intent.GetIntExtra("songID", -1);
            string title = this.Intent.GetStringExtra("songTitle");
            string artist = this.Intent.GetStringExtra("songArtist");
            titleSong.SetText(" " + title, TextView.BufferType.Normal);
            artistSong.SetText(" " + artist, TextView.BufferType.Normal);
            
            //Se Inician los botones


            //Si la canción ya está siendo escuchada, me la continua y no me la reniciia.
            imgUrlAlbum(urlAlbum);
            if (songID == MediaPlayerRegistry.currentSong){
                _player = MediaPlayerRegistry.currentPlayer;
                continueAudio(urlAudio);
            }else{
                playAudio(urlAudio);
                MediaPlayerRegistry.currentSong = songID;
            }
            
        }

        private void continueAudio(String urlAudio)
        {
            mediaController.SetMediaPlayer(this);
            mediaController.SetAnchorView(FindViewById(Resource.Id.linearLayout));
            _player.Start();
        }

        private void playAudio(String urlAudio){
            mediaController.SetMediaPlayer(this);
            mediaController.SetAnchorView(FindViewById(Resource.Id.linearLayout));
            //Next-Previous
            mediaController.SetPrevNextListeners(this, this);
            _player.SetDataSource(urlAudio);
            _player.Prepare();
            _player.Start();
            MediaPlayerRegistry.currentPlayer = _player;

        }


        private void imgUrlAlbum(String imgUrlAlbum)
        {
            if (imgUrlAlbum == null)
            {
                imgAlbum.SetImageResource(Resource.Drawable.auriculares);

            }
            else
            {
                File imgFile = new File(imgUrlAlbum);
                if (imgFile.Exists())
                {
                    Bitmap myBitmap = BitmapFactory.DecodeFile(imgFile.AbsolutePath);
                    imgAlbum.SetImageBitmap(myBitmap);
                }
            }
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action) {
                case MotionEventActions.Down:
                    mediaController.Show();
                    break;
              
            }

            return true;
        }

        public bool CanPause()
        {
            return true;
        }

        public bool CanSeekBackward()
        {
            return true;
        }

        public bool CanSeekForward()
        {
            return true;
        }

        

        public void Pause()
        {
            _player.Pause();
        }

        public void SeekTo(int pos)
        {
            _player.SeekTo(pos);
        }

        public void Start()
        {
            _player.Start();
        }

        public void Next()
        {
            
        }

        public void Prev(View v)
        {
            
        }

        public void OnClick(View v)
        {
            string G = v.ToString();
            char m = G[G.Length-2];
           
            if (m == 't'){

            }else{

            }
        }
    }
}