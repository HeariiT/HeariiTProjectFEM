﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace heariit_ma.models
{
    public class AudioAdapter : BaseAdapter{
        Activity context;
        public List<Datos> items;
        public AudioAdapter(Activity context, List<Datos> items) : base(){
            this.context = context;
            this.items = items;
        }

        public override int Count {
            get{
                return items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position){
            return position;
        }

        public override long GetItemId(int position){
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent){
            var item = items[position];

            var view = (convertView ??
                context.LayoutInflater.Inflate(
                    Resource.Layout.ItemRow,
                    parent,
                    false)) as LinearLayout;

            var title = view.FindViewById(Resource.Id.title) as TextView;
            var artist = view.FindViewById(Resource.Id.artist) as TextView;
            var category = view.FindViewById(Resource.Id.CategoryText) as TextView;

            title.SetText(" " + item.Title, TextView.BufferType.Normal);
            artist.SetText(" " + item.Artist, TextView.BufferType.Normal);
            category.SetText(" " + item.Category, TextView.BufferType.Normal);
            return view;
        }

        public Datos GetItemAtPosition(int position) {
            return items[position];
        }
    }
}