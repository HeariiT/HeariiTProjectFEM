using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.Widget;

using heariit_ma.Fragments;

namespace heariit_ma.Activities
{
    [Activity(Label = "Main", MainLauncher = true, Theme = "@style/MyTheme")]
    public class Main : ActionBarActivity
    {
        private SupportToolbar mToolbar;
        private MyActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mLeftDrawer;

        private static readonly string[] Sections = new[] {
            "Music", "Upload"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Navigator);
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawner);

            SetSupportActionBar(mToolbar);

            mDrawerToggle = new MyActionBarDrawerToggle(
                 this,                          //Host Activity
                 mDrawerLayout,                 //DrawerLayout
                 Resource.String.openDrawer,    //Openend Message
                 Resource.String.closeDrawer    //Closed Message
                );

            mDrawerLayout.SetDrawerListener(mDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mDrawerToggle.SyncState();

   
            //Create Adapter for drawer List
            this.mLeftDrawer.Adapter = new ArrayAdapter<string>(this, Resource.Layout.ItemMenu, Sections);

            //Set click handler when item is selected
            this.mLeftDrawer.ItemClick += (sender, args) => ListItemClicked(args.Position);

            if (savedInstanceState == null)
            {
                ListItemClicked(0);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            mDrawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        private void ListItemClicked(int position)
        {
            Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = new ListSongs();
                    break;
                case 1:
                    fragment = new UploadSongs();
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            this.mLeftDrawer.SetItemChecked(position, true);
            //SupportActionBar.Title = this.title = Sections[position];
            this.mDrawerLayout.CloseDrawers();
        }
    }
}