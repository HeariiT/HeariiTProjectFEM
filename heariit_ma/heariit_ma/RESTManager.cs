using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace heariit_ma
{
    public class RESTManager : IRestService
    {
        public static string BackendAddress = "https://4d147f72-b6a3-4f80-a8ee-fb896184ecf5.mock.pstmn.io/";
        public static string X_access_token = null;

        public static string UrlSignIn = BackendAddress + "/sign_in";
        public static string UrlSignUp = BackendAddress + "/sign_up";
        public static string UrlMyProfile = BackendAddress + "/my";

        public static bool isActive()
        {
            return X_access_token == null ? false : true;
        }
        public static void setToken(string NewToken)
        {
            X_access_token = NewToken;
        }

        public async Task<string> Login(string username, string password)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(UrlSignIn);
            string jsonBody = @"{ username : " + username + ", password : " + password + "}";
        }

    }
}