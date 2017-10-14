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

namespace heariit_ma
{
    [Activity(Label = "HeariiT - Upload a song", Icon = "@drawable/icon")]
    public class Uploader : Activity
    {
        public static readonly int PickSongId = 1000;
        private Android.Net.Uri uri;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UploadLayout);

            TextView Title = FindViewById<TextView>(Resource.Id.uploadTitleText);
            TextView Album = FindViewById<TextView>(Resource.Id.uploadAlbumText);
            TextView Author = FindViewById<TextView>(Resource.Id.uploadAuthorText);

            Button PickerBtn = FindViewById<Button>(Resource.Id.uploadPickerBtn);
            Button UploadBtn = FindViewById<Button>(Resource.Id.uploadPickerBtn);
            Button BackBtn = FindViewById<Button>(Resource.Id.uploadPickerBtn);

            PickerBtn.Click += delegate
            {
                Intent intent = new Intent();
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(intent, "Pick a song"), PickSongId);

                Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA " + uri);
            };

            BackBtn.Click += delegate
            {
                var MainActivity = new Intent(this, typeof(MainActivity));
                this.StartActivity(MainActivity);
                this.Finish();
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            ICursor cursor = null;
            try
            {
                // assuming image
                var docID = DocumentsContract.GetDocumentId(data.Data);
                var id = docID.Split(':')[1];
                var whereSelect = MediaStore.Images.ImageColumns.Id + "=?";
                var projections = new string[] { MediaStore.Images.ImageColumns.Data };
                // Try internal storage first
                cursor = ContentResolver.Query(MediaStore.Images.Media.InternalContentUri, projections, whereSelect, new string[] { id }, null);
                if (cursor.Count == 0)
                {
                    // not found on internal storage, try external storage
                    cursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, projections, whereSelect, new string[] { id }, null);
                }
                var colData = cursor.GetColumnIndexOrThrow(MediaStore.Images.ImageColumns.Data);
                cursor.MoveToFirst();
                var fullPathToImage = cursor.GetString(colData);
                Console.WriteLine("MediaPath", fullPathToImage);
            }
            catch (Exception err)
            {
                Console.WriteLine("MediaPath", err.Message);
            }
            finally
            {
                cursor?.Close();
                cursor?.Dispose();
            }
        }
    }
}