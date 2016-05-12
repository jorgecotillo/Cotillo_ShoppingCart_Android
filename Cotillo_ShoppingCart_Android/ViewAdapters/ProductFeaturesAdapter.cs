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
using Cotillo_ShoppingCart_Android.TableItems;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using Cotillo_ShoppingCart_Models;

namespace Cotillo_ShoppingCart_Android.ViewAdapters
{
    public class ProductFeaturesAdapter : BaseAdapter<CommonItem>
    {
        List<CommonItem> _items;
        Activity _context;
        public ProductFeaturesAdapter(Activity context, List<CommonItem> items)
            : base()
        {
            this._context = context;
            this._items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override CommonItem this[int position]
        {
            get { return _items[position]; }
        }
        public override int Count
        {
            get { return _items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            // SIMPLE LIST ITEM 1
            //View view = convertView;
            //if (view == null)
            //    view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM 2
            View view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            // SIMPLE SELECTABLE LIST ITEM
            // View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleSelectableListItem, null);
            // view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM ACTIVATED 1
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated1, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM ACTIVATED 2
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated2, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            // SIMPLE LIST ITEM CHECKED
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemChecked, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM MULTIPLE CHOICE
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemMultipleChoice, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // SIMPLE LIST ITEM SINGLE CHOICE
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemSingleChoice, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            // TWO LINE LIST ITEM
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

            // ACTIVITY LIST ITEM
            //View view = _context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
            //view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.ImageResourceId);

            // TEST LIST ITEM
            //View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TestListItem, null);
            //view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;

            return view;
        }
    }
}