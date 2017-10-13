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
            BackendAddress = "http://192.168.43.249:4000";
            X_access_token = null;
            client = new RestClient(BackendAddress);
        }

        public bool isActive()
        {
            return X_access_token == null ? false : true;
        }

        /**
         *  string - Es la cadena mostrada en los Toast como respuesta a la peticion
         *  RESTManager - un objeto tipo RESTManager, se usa para realizar todas las peticiones
        **/
        public KeyValuePair<string, RESTManager> SignIn(string Email, string Password)
        {
            //Request method and parameters
            var request = new RestRequest(UrlSignIn, Method.POST);
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            //request.AddParameter("email", "luergica2@gmail.com");
            //request.AddParameter("password", "12345678");
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse response = client.Execute(request);
                
                //IRestResponse<Data> response = client.Execute<Data>(request);
                if(response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Successful sign in (200)
                {
                    X_access_token = response.Headers.ToList().Find(x => x.Name == "x-access-token").Value.ToString();
                    RootUserData user = JsonConvert.DeserializeObject<RootUserData>(response.Content);
                    
                    CurrentUser.id = user.data.id;
                    CurrentUser.email = user.data.email;
                    CurrentUser.first_name = user.data.first_name;
                    CurrentUser.last_name = user.data.last_name;
                    CurrentUser.username = user.data.username;
                    CurrentUser.x_access_token = X_access_token;

                    return new KeyValuePair<string, RESTManager>( Application.Context.Resources.GetString(Resource.String.welcome_message) + " " + CurrentUser.first_name , this);
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized)) //Unauthorized (401)
                {
                    return new KeyValuePair<string, RESTManager>( Application.Context.Resources.GetString(Resource.String.failed_credentials), this );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@SignIn method: " + ex.Message);
                return new KeyValuePair<string, RESTManager>( Application.Context.Resources.GetString(Resource.String.bad_connection), this);
            }
            return new KeyValuePair<string, RESTManager>( Application.Context.Resources.GetString(Resource.String.bad_connection), this);
        }

        /**
         *  List<string> - Lista de cadenas para ser mostradas en un Toast
        **/
        public KeyValuePair<List<string>, bool> SignUp( string email, string password, string username, string first_name, string last_name)
        {
            //Request method and parameters
            var request = new RestRequest(UrlSignUp, Method.POST);
            request.AddParameter("email", email);
            request.AddParameter("password", password);
            request.AddParameter("username", username);
            request.AddParameter("first_name", first_name);
            request.AddParameter("last_name", last_name);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse response = client.Execute(request);
                RootSignUpData user = null;

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Successful sign up (200)
                {
                    user = JsonConvert.DeserializeObject<RootSignUpData>(response.Content);
                    List<string> oks = new List<string> { Application.Context.Resources.GetString(Resource.String.signup_message) };
                    return new KeyValuePair<List<string>, bool>(oks, true);
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest)) //Bad Request (400)
                {
                    List<string> err = new List<string>(user.errors.full_messages);
                    return new KeyValuePair<List<string>, bool>(err, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@SignUp method: " + ex.Message);
                List<string> errors = new List<string> { "EXCEPION! " + Application.Context.Resources.GetString(Resource.String.bad_connection) };
                return new KeyValuePair<List<string>, bool>(errors, false);
            }
            List<string> strings = new List<string> { Application.Context.Resources.GetString(Resource.String.unexpected_error) };
            return new KeyValuePair<List<string>, bool>(strings, false);
        }



    }
}