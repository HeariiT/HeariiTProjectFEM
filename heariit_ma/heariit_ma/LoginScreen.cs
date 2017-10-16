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
using Android.Preferences;

namespace heariit_ma
{
    [Activity(Label = "HeariiT", Icon = "@drawable/icon")]
    public class LoginScreen : Activity
    {
        private bool Validated = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            RESTManager manager = new RESTManager();
            /**
             * Intenta recuperar un string guardado localmente, corresponde al token de sesion
             * si es valido, dirige al mainactivity, si no, al login.
             * **/
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            string Stored_x_access_token = prefs.GetString("x-access-token", null);

            base.OnCreate(savedInstanceState);

            if (!string.IsNullOrEmpty(Stored_x_access_token) && manager.ValidToken(Stored_x_access_token) && true)
            {
                var mainActivity = new Intent(this, typeof(MainActivity));
                mainActivity.PutExtra("x-access-token", Stored_x_access_token);
                this.StartActivity(mainActivity);
                this.Finish();
            }
            else
            {
                SetContentView(Resource.Layout.LoginLayout);
                Button signin = FindViewById<Button>(Resource.Id.loginButtonSignIn);
                Button signup = FindViewById<Button>(Resource.Id.loginButtonSignUp);
                TextView email = FindViewById<TextView>(Resource.Id.loginEmailText);
                TextView pass = FindViewById<TextView>(Resource.Id.loginPasswordText);

                signin.Click += delegate
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

                    if (Validated)
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
                            mainActivity.PutExtra("x-access-token", response.Value);
                            this.StartActivity(mainActivity);
                            this.Finish();
                        }
                    }

                };

                signup.Click += delegate
                {
                    var SignupScreen = new Intent(this, typeof(SignUpScreen));
                    this.StartActivity(SignupScreen);
                    this.Finish();
                };
            }
        }
    }
}