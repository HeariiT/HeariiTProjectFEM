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
using RestSharp;
using heariit_ma.models;
using Android.Content.Res;
using Newtonsoft.Json;

namespace heariit_ma
{
    public class RESTManager
    {
        private string BackendAddress { set; get; }
        public string X_access_token { set; get; }

        private const string UrlSignIn = "/sign_in";
        private const string UrlSignUp = "/sign_up";
        private const string UrlMyProfile = "/my";

        private RestClient client;

        public RESTManager()
        {
            this.BackendAddress = "https://4d147f72-b6a3-4f80-a8ee-fb896184ecf5.mock.pstmn.io/";
            this.X_access_token = null;
            this.client = new RestClient(BackendAddress);
        }

        public bool isActive()
        {
            return X_access_token == null ? false : true;
        }
        
        public KeyValuePair<string, string> SignIn(User user)
        {
            //Request method and parameters
            var request = new RestRequest(UrlSignIn, Method.POST);
            request.AddParameter("email", user.Email);
            request.AddParameter("password", user.Password);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse<User> response = client.Execute<User>(request);
                if(response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Successful sign in (200)
                {
                    X_access_token = response.Headers.ToList().Find(x => x.Name == "x-access-token").Value.ToString();
                    return new KeyValuePair<string, string>( Application.Context.Resources.GetString(Resource.String.welcome_message) , X_access_token);
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized)) //Unauthorized (401)
                {
                    return new KeyValuePair<string, string>( Application.Context.Resources.GetString(Resource.String.failed_credentials), null );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@SignIn method: " + ex.Message);
                return new KeyValuePair<string, string>( Application.Context.Resources.GetString(Resource.String.bad_connection), null);
            }
            return new KeyValuePair<string, string>( Application.Context.Resources.GetString(Resource.String.bad_connection), null);
        }   
        
    }
}