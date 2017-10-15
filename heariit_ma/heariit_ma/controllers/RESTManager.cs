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
using System.IO;

namespace heariit_ma
{
    public class RESTManager
    {
        private string BackendAddress { set; get; }
        public string X_access_token { set; get; }

        private const string UrlSignIn = "/sign_in";
        private const string UrlSignUp = "/sign_up";
        private const string UrlMyProfile = "/my";
        private const string UrlValidateEmail = "/email";
        private const string UrlValidateUsername = "/username";
        private const string UrlValidateToken = "/validate";
        private const string UrlMySongs = "/songs";
        private const string UrlDownloadSongs = "/download/";
        private const string UrlSongGategory = "/category_for_file/";
        private const string UrlMatch = "/match";
        private const string UrlUserCategories = "/user_categories";

        private RestClient client;

        public RESTManager()
        {
            BackendAddress = "http://192.168.0.20:4000";
            X_access_token = null;
            client = new RestClient(BackendAddress);
        }

        public bool isActive()
        {
            return X_access_token == null ? false : true;
        }

        /**
         *  string - Es la cadena mostrada en los Toast como respuesta a la peticion
         *  string - es el x-access-token, se usa para realizar todas las peticiones
        **/
        public KeyValuePair<string, string> SignIn(string Email, string Password)
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
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Successful sign in (200)
                {
                    X_access_token = response.Headers.ToList().Find(x => x.Name == "x-access-token").Value.ToString();
                    RootUserData user = JsonConvert.DeserializeObject<RootUserData>(response.Content);

                    CurrentUser.id = user.data.id ?? default(int);
                    CurrentUser.email = user.data.email;
                    CurrentUser.first_name = user.data.first_name;
                    CurrentUser.last_name = user.data.last_name;
                    CurrentUser.username = user.data.username;
                    CurrentUser.x_access_token = X_access_token;

                    return new KeyValuePair<string, string>(Application.Context.Resources.GetString(Resource.String.welcome_message) + " " + CurrentUser.first_name, X_access_token);
                }
                else if (response.StatusCode.Equals(System.Net.HttpStatusCode.Unauthorized)) //Unauthorized (401)
                {
                    return new KeyValuePair<string, string>(Application.Context.Resources.GetString(Resource.String.failed_credentials), null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@SignIn method: " + ex.Message);
                return new KeyValuePair<string, string>(Application.Context.Resources.GetString(Resource.String.bad_connection), null);
            }
            return new KeyValuePair<string, string>(Application.Context.Resources.GetString(Resource.String.bad_connection), null);
        }

        /**
         *  List<string> - Lista de cadenas para ser mostradas en un Toast
        **/
        public KeyValuePair<List<string>, bool> SignUp(string Email, string Password, string Username, string FirstName, string LastName)
        {
            //Request method and parameters
            var request = new RestRequest(UrlSignUp, Method.POST);
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            request.AddParameter("username", Username);
            request.AddParameter("first_name", FirstName);
            request.AddParameter("last_name", LastName);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse response = client.Execute(request);
                RootSignUpData user = JsonConvert.DeserializeObject<RootSignUpData>(response.Content);

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Successful sign up (200)
                {
                    List<string> Success = new List<string> { Application.Context.Resources.GetString(Resource.String.signup_message) };
                    return new KeyValuePair<List<string>, bool>(Success, true);
                }
                else if (response.StatusCode.ToString().Equals("422")) //Unprocessable Entity (422)
                {
                    List<string> Errors = new List<string>(user.errors.full_messages);
                    return new KeyValuePair<List<string>, bool>(Errors, false);
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

        /**
         *  bool - verdadero si el email suministrado esta disponible
        **/
        public bool AvailableEmail(string Email)
        {
            //Request method and parameters
            var request = new RestRequest(UrlValidateEmail, Method.POST);
            request.AddParameter("email", Email);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse response = client.Execute(request);

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.NotFound)) //Bad Request (400)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@ValidateEmail method: " + ex.Message);
                return false;
            }
            return false;
        }

        /**
         * bool - verdadero si el username suministrado esta disponible
        **/
        public bool AvailableUsername(string Username)
        {
            //Request method and parameters
            var request = new RestRequest(UrlValidateUsername, Method.POST);
            request.AddParameter("username", Username);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            //Response
            try
            {
                IRestResponse response = client.Execute(request);

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.NotFound)) //Bad Request (400)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@ValidateUsername method: " + ex.Message);
                return false;
            }
            return false;
        }

        /**
         * bool - verdadero si el token suministrado es valido
        **/
        public bool ValidToken(string Token)
        {
            //Request method and parameters
            var request = new RestRequest(UrlValidateToken, Method.POST);
            //Headers
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("x-access-token", Token);
            //Response
            try
            {
                IRestResponse response = client.Execute(request);

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) //Ok (200)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@ValidateUsername method: " + ex.Message);
                return false;
            }
            return false;
        }

        /**
         * Arreglo de objetos SongInfo
        **/
        public SongInfo[] MySongs() {

            SongInfo[] MySongs;
            var request = new RestRequest(UrlMySongs, Method.GET);
            request.AddHeader("x-access-token", CurrentUser.x_access_token);
            try
            {
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    string MyString = response.Content.ToString();
                    if (!string.IsNullOrEmpty(MyString))
                    {
                        Console.WriteLine(MyString);
                        MySongs = Newtonsoft.Json.JsonConvert.DeserializeObject<SongInfo[]>(MyString);
                        return MySongs;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@MySongs method: " + ex.Message);
            }
            return new SongInfo[0];

        }


        public string getASong(string id)
        {
            string SongDownload = UrlDownloadSongs + id;
            string Filename = null;
            var request = new RestRequest(SongDownload, Method.GET);
            request.AddHeader("x-access-token", CurrentUser.x_access_token);
            try
            {
                IRestResponse response = client.Execute(request);
                Console.WriteLine("AAAAAAAAAAAAAAAAAAAA");
                Console.WriteLine(response.StatusCode);
                Console.WriteLine("BBBBBBBBBBBBBBBBBBBB");
                Console.WriteLine(response.Headers);
                Console.WriteLine("CCCCCCCCCCCCCCCCCCCC");
                Console.WriteLine(response.Content);
                Console.WriteLine("DDDDDDDDDDDDDDDDDDDD");
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    Filename = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp3";
                    byte[] bytes = response.RawBytes;
                    File.WriteAllBytes(Filename, bytes);
                    return Filename;
                }
                else
                {
                    Console.WriteLine("NO SE PUDO LPTM");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@getASong method: " + ex.Message);
            }
            return null;
        }

        /*
         *Retorna la categoría de una canción 
        */
        public string getSongCategory(string id)
        {

            string SongCategory = UrlSongGategory + id;
            var request = new RestRequest(SongCategory, Method.GET);
            string category = "";
            request.AddHeader("x-access-token", CurrentUser.x_access_token);
            try
            {
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    string MyString = response.Content.ToString();
                    CategoryData cat = new CategoryData();
                    cat = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoryData>(MyString);
                    if (!string.IsNullOrEmpty(cat.category_name))
                    {
                        category = cat.category_name;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@getSongCategory method: " + ex.Message);
            }
            return category;
        }


        /*
         *Retorna las categorías de un usuario
        */

        public CategoryData[] getMyCategories() {
            var request = new RestRequest(UrlUserCategories, Method.GET);
            request.AddHeader("x-access-token", CurrentUser.x_access_token);

            try
            {
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    string MyString = response.Content.ToString();
                    CategoryData[] cat;
                    if (!string.IsNullOrEmpty(MyString))
                    {
                        Console.WriteLine(MyString);
                        cat = Newtonsoft.Json.JsonConvert.DeserializeObject<CategoryData[]>(MyString);
                        return cat;
                    }
                }
                else
                {
                    Console.WriteLine("No Acepted at RESTManaher@getMyCategories, Status: " + response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@getMyCategories method: " + ex.Message);
            }

            return new CategoryData[0];
        }
        public class matchObj
        {
            public string category_id { get; set; }
            public int file_id { get; set; }
        }
        public bool Match(int song_id, string category_id, bool new_match){
            RestRequest request; 
            if (new_match)
            {
                request = new RestRequest(UrlMatch, Method.POST);
            }
            else
            {
                request = new RestRequest(UrlMatch, Method.PUT);
            }

            
            request.AddHeader("x-access-token", CurrentUser.x_access_token);
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("category_id", category_id, ParameterType.RequestBody);
            //request.AddParameter("file_id", (int)song_id, ParameterType.RequestBody);
            var req = new matchObj { category_id = category_id, file_id = song_id };
            request.RequestFormat = DataFormat.Json;
            request.AddBody(req);
            
            Console.WriteLine(request.GetHashCode());

            try
            {
                IRestResponse response = client.Execute(request);
                
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.Created)|| 
                    response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) {
                    return true;
                }
                else
                {
                    Console.WriteLine("No Acepted at RESTManager@Match, Status: " + response.StatusCode.ToString());
                    Console.WriteLine(response.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@Match method: " + ex.Message);

            }
            return false;
        }

        public bool DeleteMatch(int id)
        {
            var request = new RestRequest(UrlMatch+"/"+id.ToString(), Method.DELETE);
            request.AddHeader("x-access-token", CurrentUser.x_access_token);
            try
            {
                IRestResponse response = client.Execute(request);

                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("No Acepted at RESTManager@DeleteMatch, Status: " + response.StatusCode.ToString());
                    Console.WriteLine(response.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at RESTManager@DeleteMatch method: " + ex.Message);
            }
            
            return false;
        }
    }

}