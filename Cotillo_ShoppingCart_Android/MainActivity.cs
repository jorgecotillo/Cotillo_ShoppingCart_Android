using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Cotillo_ShoppingCart_Android.TableItems;
using Cotillo_ShoppingCart_Android.ViewAdapters;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = true, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class MainActivity : CommonListActivity
    {
        List<CommonItem> _items;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _items = new List<CommonItem>()
            {
                new CommonItem() { Heading = "Hola", SubHeading = "Coco", ImageResourceId = Resource.Drawable.Icon },
                new CommonItem() { Heading = "Coco", SubHeading = "Hola", ImageResourceId = Resource.Drawable.Icon }
            };

            ListView.ChoiceMode = ChoiceMode.Single;
            
            //This line is needed so that the ListView can use the datasource to display the items.
            //Inspect ProductFeaturesAdapter and the code there shows which built-in layout is being used
            ListAdapter = new ProductFeaturesAdapter(this, _items);

            ListView.ItemClick += ListView_ItemClick;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(_items != null && _items.Count > 0)
            {
                var listView = sender as ListView;
                var t = _items[e.Position];
                Android.Widget.Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}

