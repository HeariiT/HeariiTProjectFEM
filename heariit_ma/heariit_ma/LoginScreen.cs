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

namespace heariit_ma
{
    [Activity(Label = "LoginScreen", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginLayout);
            Button login = FindViewById<Button>(Resource.Id.loginButtonSignIn);
            login.Click += (object sender, EventArgs e) =>
            {
                Android.Widget.Toast.MakeText(this, "Login button for: " + Resource.Id.loginEmailText.ToString, Android.Widget.ToastLength.Short).Show();
            };
        }
    }
}