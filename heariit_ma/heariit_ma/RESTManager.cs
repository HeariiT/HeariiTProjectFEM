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
using System.Threading.Tasks;

namespace heariit_ma
{
    public class RESTManager
    {

        private string BackendAddress { set; get; }
        public string x_api_key { set; get; }

        private const string UrlSongs = "/songs";

        private RestClient client;

        public RESTManager() {
            BackendAddress = "https://5d15a108-ff23-410c-b597-153e8a92310f.mock.pstmn.io";
            x_api_key = "ad807eef1bbd4a6a90ba5688f01cea8b";
            client = new RestClient(BackendAddress);
        }


        public String GET(){

            var request = new RestRequest(UrlSongs, Method.GET);
            request.AddHeader("x-api-key", x_api_key);
            request.AddHeader("x-mock-response-code", "200");
            try
            {
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.Equals(System.Net.HttpStatusCode.OK)) {
                    Console.WriteLine(response.Content);
                    return response.Content.ToString();
                }
                else
                {
                    Console.WriteLine("Not Found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                Console.WriteLine("Exception at RESTManager@SignIn method: " + ex.Message);
                Console.WriteLine("REEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                
            }

            return null;

        }
    }
}