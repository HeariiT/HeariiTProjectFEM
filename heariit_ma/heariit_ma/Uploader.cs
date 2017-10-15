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
using Android.Database;
using Android.Provider;
using heariit_ma.models;

namespace heariit_ma
{
    [Activity(Label = "HeariiT - Upload a song", Icon = "@drawable/icon")]
    public class Uploader : Activity
    {
        private static readonly int PickSongId = 1000;
        private string Path;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UploadLayout);

            TextView Title = FindViewById<TextView>(Resource.Id.uploadTitleText);
            TextView Album = FindViewById<TextView>(Resource.Id.uploadAlbumText);
            TextView Author = FindViewById<TextView>(Resource.Id.uploadAuthorText);

            Button PickerBtn = FindViewById<Button>(Resource.Id.uploadPickerBtn);
            Button UploadBtn = FindViewById<Button>(Resource.Id.uploadUploadBtn);
            Button BackBtn = FindViewById<Button>(Resource.Id.uploadBackBtn);

            PickerBtn.Click += delegate
            {
                Intent intent = new Intent();
                intent.SetType("audio/mp3");
                intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(intent, "Pick a song"), PickSongId);
            };

            UploadBtn.Click += delegate
            {
                KeyValuePair<string, bool> Valid = ValidateFields(Title.Text, Author.Text, Album.Text);
                if ( Valid.Value )
                {
                    RESTManager client = new RESTManager();
                    KeyValuePair<string, bool> response = client.UploadSong(Title.Text, Album.Text, Author.Text, Path);
                    if (response.Value)
                    {
                        var MainActivity = new Intent(this, typeof(MainActivity));
                        MainActivity.PutExtra("x-access-token", CurrentUser.x_access_token);
                        this.StartActivity(MainActivity);
                        this.Finish();
                    }
                    Toast.MakeText(this, response.Key, ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, Valid.Key, ToastLength.Long).Show();
                }
                
            };

            BackBtn.Click += delegate
            {
                var MainActivity = new Intent(this, typeof(MainActivity));
                MainActivity.PutExtra("x-access-token", CurrentUser.x_access_token);
                this.StartActivity(MainActivity);
                this.Finish();
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && data != null && requestCode == PickSongId)
            {
                Button Picker = FindViewById<Button>(Resource.Id.uploadPickerBtn);
                Button Upload = FindViewById<Button>(Resource.Id.uploadUploadBtn);

                string Path = GetRealPathFromURI(data.Data);
                this.Path = Path;
                string Filename = System.IO.Path.GetFileName(Path);
                Picker.Text = Filename;
                Upload.Enabled = true;
            }
        }

        private string GetRealPathFromURI(Android.Net.Uri uri)
        {
            string doc_id = "";
            using (var c1 = ContentResolver.Query(uri, null, null, null, null))
            {
                c1.MoveToFirst();
                String document_id = c1.GetString(0);
                doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
            }

            string path = null;

            // The projection contains the columns we want to return in our query.
            string selection = MediaStore.Audio.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = ManagedQuery(MediaStore.Audio.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
            {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Audio.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }
            return path;
        }

        private KeyValuePair<string, bool> ValidateFields(string Title, string Author, string Album)
        {
            string Message = null;
            bool Valid = false;

            if (!string.IsNullOrEmpty(Title))
            {
                if (!string.IsNullOrEmpty(Author))
                {
                    if (!string.IsNullOrEmpty(Album))
                    {
                        Valid = true;
                    }
                    else
                    {
                        Message = "Album " + Application.Context.Resources.GetString(Resource.String.empty_field);
                    }
                }
                else
                {
                    Message = "Author " + Application.Context.Resources.GetString(Resource.String.empty_field);
                }
            }
            else
            {
                Message = "Title " + Application.Context.Resources.GetString(Resource.String.empty_field);
            }

            return new KeyValuePair<string, bool>(Message, Valid);
        }
    }
}