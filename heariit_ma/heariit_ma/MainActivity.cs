using Android.App;
using Android.Widget;
using Android.OS;

namespace heariit_ma
{
    [Activity(Label = "heariit_ma", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}

