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
    [Activity(Label = "HeariiT")]
    public class SignUpScreen : Activity
    {
        private bool Validated = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUpLayout);

            TextView FirstName = FindViewById<TextView>(Resource.Id.signupFirstNameText);
            TextView LastName = FindViewById<TextView>(Resource.Id.signupLastNameText);
            TextView Username = FindViewById<TextView>(Resource.Id.signupUsernameText);
            TextView Email = FindViewById<TextView>(Resource.Id.signupEmailText);
            TextView Password1 = FindViewById<TextView>(Resource.Id.signupPasswordText1);
            TextView Password2 = FindViewById<TextView>(Resource.Id.signupPasswordText2);

            Button SignupButton = FindViewById<Button>(Resource.Id.signupSignUpBtn);
            Button ClearButton = FindViewById<Button>(Resource.Id.signupClearBtn);
            Button BackButton = FindViewById<Button>(Resource.Id.signupBackBtn);

            SignupButton.Click += delegate
            {
                if (!Android.Util.Patterns.EmailAddress.Matcher(Email.Text).Matches())
                {
                    Toast.MakeText(this, Application.Context.Resources.GetString(Resource.String.bad_email), ToastLength.Long).Show();
                }
                else if (Password1.Text != Password2.Text)
                {
                    Toast.MakeText(this, "Password: " + Application.Context.Resources.GetString(Resource.String.error_password_not_equal), ToastLength.Long).Show();
                }
                else
                {
                    Validated = true;
                }

                if (Validated)
                {
                    RESTManager manager = new RESTManager();
                    var response = manager.SignUp(Email.Text, Password1.Text, Username.Text, FirstName.Text, LastName.Text);

                    if ( !response.Value ) //Fallo la creacion de usuario
                    {
                        Toast.MakeText(this, response.Key[0], ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, response.Key[0], ToastLength.Long).Show();

                        var LoginScreen = new Intent(this, typeof(LoginScreen));
                        this.StartActivity(LoginScreen);
                        this.Finish();
                    }
                }
            };

            ClearButton.Click += delegate
            {
                FirstName.Text = "";
                LastName.Text = "";
                Username.Text = "";
                Email.Text = "";
                Password1.Text = "";
                Password2.Text = "";
            };

            BackButton.Click += delegate
            {
                var SigninScreen = new Intent(this, typeof(LoginScreen));
                this.StartActivity(SigninScreen);
                this.Finish();
            };

        }
    }
}