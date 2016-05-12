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
using Cotillo_ShoppingCart_Models;
using System.Net.Http;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = true, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class MainActivity : CommonListActivity
    {
        List<CommonItem> _items;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                var list = await
                            Helper.MobileService.InvokeApiAsync<List<CategorySummaryModel>>("v1/category/summary-list", HttpMethod.Get, null);
                _items = new List<CommonItem>();

                foreach (var item in list)
                {
                    _items.Add(new CommonItem() { Heading = $"Category: {item.CategoryName}", SubHeading = item.Location });
                }

                ListView.ChoiceMode = ChoiceMode.Single;

                //This line is needed so that the ListView can use the datasource to display the items.
                //Inspect ProductFeaturesAdapter and the code there shows which built-in layout is being used
                ListAdapter = new ListFeaturesAdapter(this, _items);

                ListView.ItemClick += ListView_ItemClick;
            }
            catch (Exception ex)
            {
                //Log error
                Toast.MakeText(this, "An error occurred while processing your request", ToastLength.Long);
            }
            finally
            {
                progress.Dismiss();
            }
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if(_items != null && _items.Count > 0)
            {
                var listView = sender as ListView;
                var t = _items[e.Position];



                Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}

