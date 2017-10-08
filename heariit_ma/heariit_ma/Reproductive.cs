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
    [Activity(Label = "HeariiT Player")]
    public class Reproductive : Activity, MediaController.IMediaPlayerControl, IOnTouchListener, IOnClickListener {

        MediaPlayer _player;
        MediaController mediaController;
        LinearLayout linearLayout;
        ImageView imgAlbum;
        TextView titleSong;
        TextView artistSong;
        int listSize;
        int songID;
        String urlAlbum;
        String urlAudio;
        string title;
        string artist;

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
            _player = new MediaPlayer();
            mediaController = new Android.Widget.MediaController(this, true);


            //Se leen los elementos del Intent
            urlAlbum = this.Intent.GetStringExtra("urlAlbum");
            urlAudio = this.Intent.GetStringExtra("urlAudio");
            songID = this.Intent.GetIntExtra("songID", -1);
            title = this.Intent.GetStringExtra("songTitle");
            artist = this.Intent.GetStringExtra("songArtist");
            listSize = this.Intent.GetIntExtra("listSize", -1);


            //Se Asignan los Titulos y Artista
            titleSong = FindViewById<TextView>(Resource.Id.reproductive_title);
            artistSong = FindViewById<TextView>(Resource.Id.reproductive_artist);

            


            //Establecer los elementos visuales
            setVisuals();


            //Si la canción ya está siendo escuchada, me la continua y no me la reniciia.
            
            if (songID == MediaPlayerRegistry.currentSong){
                _player = MediaPlayerRegistry.currentPlayer;
                continueAudio();
            }else{
                MediaPlayerRegistry.currentPlayer.Stop();
                playAudio();
                MediaPlayerRegistry.currentSong = songID;
            }
            
        }

        private void setVisuals() {
            titleSong.SetText(" " + title, TextView.BufferType.Normal);
            artistSong.SetText(" " + artist, TextView.BufferType.Normal);
            imgUrlAlbum(urlAlbum);
        }

        private void continueAudio() {
            mediaController.SetMediaPlayer(this);
            mediaController.SetAnchorView(FindViewById(Resource.Id.linearLayout));
            mediaController.SetPrevNextListeners(this, this);
            _player.Start();
            _player.Completion += delegate
            {
                ChangeSongBack();
            };
        }

        private void playAudio(){
            mediaController.SetMediaPlayer(this);
            mediaController.SetAnchorView(FindViewById(Resource.Id.linearLayout));
            mediaController.SetPrevNextListeners(this, this);

            _player.SetDataSource(urlAudio);
            _player.Prepare();
            _player.Start();
            _player.Completion += delegate
            {
                ChangeSong(1);
            };
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
       
        public void ChangeSong(int N)
        {
            N = songID + N;
            if (N == listSize){
                N = 0;
            }else if(N<0){
                N = listSize - 1;
            }
            _player.Stop();
            _player.Release();
            _player = new MediaPlayer();
            songID = N;
            MediaPlayerRegistry.currentSong = songID;
            String[] NewSong = MediaPlayerRegistry.Songs[N];
            urlAlbum = NewSong[0];
            urlAudio = NewSong[1];
            title = NewSong[2];
            artist = NewSong[3];
            setVisuals();
            playAudio();
        }

        public void ChangeSongBack()
        {
            int N = songID + 1;
            if (N == listSize){
                N = 0;
            }
            MediaPlayerRegistry.currentPlayer.Stop();
            MediaPlayerRegistry.currentPlayer.Release();
            _player = new MediaPlayer();
            songID = N;
            MediaPlayerRegistry.currentSong = songID;
            String[] NewSong = MediaPlayerRegistry.Songs[N];
            urlAlbum = NewSong[0];
            urlAudio = NewSong[1];
            title = NewSong[2];
            artist = NewSong[3];
            setVisuals();
            playAudio();
        }

        public void OnClick(View v)
        {
            string G = v.ToString();
            char m = G[G.Length - 2];

            if (m == 't')
            {
                ChangeSong(1);
            }
            else
            {
                ChangeSong(-1);
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


    }
}