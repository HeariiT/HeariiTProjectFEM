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
        RESTManager manager = new RESTManager();

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
                KeyValuePair<string, bool> Validator = ValidateFields(Email.Text, Password1.Text, Password2.Text, Username.Text, FirstName.Text, LastName.Text);

                if ( Validator.Value ) //Campos correctos
                {
                    var response = manager.SignUp(Email.Text, Password1.Text, Username.Text, FirstName.Text, LastName.Text);

                    if ( response.Value ) //Creación de usuario exitosa
                    {
                        var LoginScreen = new Intent(this, typeof(LoginScreen));
                        this.StartActivity(LoginScreen);
                        this.Finish();
                    }

                    foreach (var item in response.Key)
                    {
                        Toast.MakeText(this, item, ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, Validator.Key, ToastLength.Long).Show();
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

        private KeyValuePair<string, bool> ValidateFields(string Email, string Password1, string Password2, string Username, string FirstName, string LastName)
        {
            string Message = null;
            bool Valid = false;

            if( !string.IsNullOrEmpty(FirstName) )
            {
                if (!string.IsNullOrEmpty(LastName))
                {
                    if (!string.IsNullOrEmpty(Username))
                    {
                        if( manager.AvailableUsername(Username))
                        {
                            if (Android.Util.Patterns.EmailAddress.Matcher(Email).Matches())
                            {
                                if (manager.AvailableEmail(Email))
                                {
                                    if (!string.IsNullOrEmpty(Password1) || !string.IsNullOrEmpty(Password2))
                                    {
                                        if (Password1.Equals(Password2))
                                        {
                                            Valid = true;
                                        }
                                        else
                                        {
                                            Message = Application.Context.Resources.GetString(Resource.String.error_password_not_equal);
                                        }
                                    }
                                    else
                                    {
                                        Message = "Password " + Application.Context.Resources.GetString(Resource.String.empty_field);
                                    }
                                }
                                else
                                {
                                    Message = "Email '" + Email + "' " + Application.Context.Resources.GetString(Resource.String.error_not_available);
                                }
                            }
                            else
                            {
                                Message = "Email " + Application.Context.Resources.GetString(Resource.String.bad_email);
                            }
                        }
                        else
                        {
                            Message = "Username '" + Username + "' " + Application.Context.Resources.GetString(Resource.String.error_not_available);
                        }
                    }
                    else
                    {
                        Message = "Username " + Application.Context.Resources.GetString(Resource.String.empty_field);
                    }
                }
                else
                {
                    Message = "Last name " + Application.Context.Resources.GetString(Resource.String.empty_field);
                }
            }
            else
            {
                Message = "First name " + Application.Context.Resources.GetString(Resource.String.empty_field);
            }
            
            return new KeyValuePair<string, bool>(Message, Valid);
        }
    }
}