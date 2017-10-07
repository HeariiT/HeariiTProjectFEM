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

namespace heariit_ma
{
    [Activity(Label = "HeariiT Music")]
    public class Reproductive : Activity{

        MediaPlayer _player;
        MediaController mediaController;
        LinearLayout linearLayout;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Reproductive);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);
            //linearLayout.SetOnTouchListener(this);
            String urlAlbum = this.Intent.GetStringExtra("urlAlbum");
            String urlAudio = this.Intent.GetStringExtra("urlAudio");
            _player = new MediaPlayer();
            mediaController = new Android.Widget.MediaController(this, true);
            playAudio(urlAudio);
        }

        private void playAudio(String urlAudio){
            _player.SetDataSource(urlAudio);
            _player.Prepare();
            _player.Start();
        }
    }
}