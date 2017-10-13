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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace heariit_ma.models
{
    [Serializable]
    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User( ) { }
        public User( string Email, string Password )
        {
            this.Email = Email;
            this.Password = Password;
        }
        public User(string Email, string Password, string Username, string FirstName, string LastName)
        {
            this.Email = Email;
            this.Password = Password;
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }
    }
}