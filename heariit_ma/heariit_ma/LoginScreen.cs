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
using heariit_ma.models;
using Newtonsoft.Json;

namespace heariit_ma
{
    [Activity(Label = "HeariiT", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginScreen : Activity
    {
        private bool Validated = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginLayout);
            Button login = FindViewById<Button>(Resource.Id.loginButtonSignIn);
            TextView email = FindViewById<TextView>(Resource.Id.loginEmailText);
            TextView pass = FindViewById<TextView>(Resource.Id.loginPasswordText);

            RESTManager manager = new RESTManager();

            login.Click += delegate
            {

                if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
                {
                    Toast.MakeText(this, Application.Context.Resources.GetString(Resource.String.bad_email), ToastLength.Long).Show();
                }
                else if (string.IsNullOrEmpty(pass.Text))
                {
                    Toast.MakeText(this, "Password: " + Application.Context.Resources.GetString(Resource.String.empty_field), ToastLength.Long).Show();
                }
                else
                {
                    Validated = true;
                }

                if ( Validated )
                {
                    var response = manager.SignIn(email.Text, pass.Text);

                    if (response.Value == null) //No token, sign in failed
                    {
                        Toast.MakeText(this, response.Key, ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, response.Key, ToastLength.Long).Show();

                        var mainActivity = new Intent(this, typeof(MainActivity));
                        mainActivity.PutExtra("RESTManager", JsonConvert.SerializeObject(response.Value));
                        this.StartActivity(mainActivity);
                        this.Finish();
                    }
                }
                
            };
        }
    }
}