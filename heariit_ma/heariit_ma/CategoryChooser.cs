using Android.App;
using Android.Widget;
using Android.OS;
using heariit_ma.models;
using System.Collections.Generic;
using Android;
using Android.Content.PM;
using Android.Provider;
using Android.Database;
using System;
using Android.Content;
using Android.Media;
using Newtonsoft.Json;
using Android.Preferences;

namespace heariit_ma
{
    [Activity(Label = "Choose Category")]
    public class CategoryChooser : Activity
    {
        List<CategoryData> items;
        ListView listData;
        CategoryAdapter MyAdapter;
        RESTManager manager = new RESTManager();
        CategoryData[] MyCategories;
        int id;
        string current_category;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            MyCategories = manager.getMyCategories();
            if (MyCategories.Length == 0)
            {
                Toast.MakeText(this, Application.Resources.GetString(Resource.String.warning_not_categories), ToastLength.Long).Show();
            }
            id = this.Intent.GetIntExtra("songId",-1);
            current_category = this.Intent.GetStringExtra("currentCategory");

            SetContentView(Resource.Layout.CategoryLayout);

            listData = FindViewById<ListView>(Resource.Id.listCategory);
            items = new List<CategoryData>();

            listData.ItemClick += (object sender, AdapterView.ItemClickEventArgs args)
                => listView_ItemClick(sender, args);
            categoryCursor();
        }

        public override void OnBackPressed()
        {
            var MainActivity = new Intent(this, typeof(MainActivity));
            MainActivity.PutExtra("x-access-token", CurrentUser.x_access_token);
            this.StartActivity(MainActivity);
            this.Finish();
        }

        void printProtm(bool succ, bool del, string name)
        {
            if (!succ)
            {
                Toast.MakeText(this, Application.Resources.GetString(Resource.String.match_fail), ToastLength.Long).Show();

            }
            else
            {
                if (del)
                {
                    Toast.MakeText(this, Application.Resources.GetString(Resource.String.match_del), ToastLength.Long).Show();
                    current_category = "";
                }
                else
                {
                    Toast.MakeText(this, Application.Resources.GetString(Resource.String.match_succ), ToastLength.Long).Show();
                    current_category = name;
                }
                
            }
            
        }

        void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs E)
        {
            int e = E.Position;
            var item = this.MyAdapter.GetItemAtPosition(e);
            string name = item.category_name;
            bool result;
            if (current_category.Equals(""))
            {
                result=manager.Match(id, item.category_id.ToString(), true);
                printProtm(result, false, name);
            }
            else if(current_category.Equals(name))
            {
                result=manager.DeleteMatch(id);
                printProtm(result, true, name);
            }
            else
            {
                result = manager.Match(id, item.category_id, false);
                printProtm(result, false, name);
            }
        }


        private void categoryCursor()
        {
            for (int i = 0; i < MyCategories.Length; i++)
            {
                CategoryData cs = MyCategories[i];
                var Title = cs.category_name;
                var id = cs.category_id;
                var er = "";
                items.Add(new CategoryData()
                {
                    category_name = Title, category_id = id, error = er
                });
            }


            listData.Adapter = MyAdapter = new CategoryAdapter(this, items);
            
        }

    }


}