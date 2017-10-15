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

namespace heariit_ma.models
{
    public class CategoryAdapter : BaseAdapter
    {
        Activity context;
        List<CategoryData> items;

        public CategoryAdapter(Activity context, List<CategoryData> items) : base(){
            this.context = context;
            this.items = items;
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            var item = items[position];

            var view = (convertView ??
                context.LayoutInflater.Inflate(
                    Resource.Layout.ItemCategory,
                    parent,
                    false)) as LinearLayout;

            var title = view.FindViewById(Resource.Id.TextCategory) as TextView;
            title.SetText(" " + item.category_name, TextView.BufferType.Normal);
            return view;
        }

        public CategoryData GetItemAtPosition(int position)
        {
            return items[position];
        }
    }
}