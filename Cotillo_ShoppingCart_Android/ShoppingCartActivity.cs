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
using Cotillo_ShoppingCart_Models;
using System.Net.Http;
using Cotillo_ShoppingCart_Android.TableItems;
using Cotillo_ShoppingCart_Android.ViewAdapters;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class ShoppingCartActivity : ListActivity
    {
        List<CommonItem> _items;

        private int ShoppingCartId { get; set; }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //Inflate the items first so it can be manipulated later
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.ShoppingCartActions, menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_checkout:
                    if (_items != null && _items.Count > 0)
                    {
                        Intent checkoutActivity = new Intent(this, typeof(CheckoutActivity));
                        var total = _items.Sum(i => i.PriceIncTax);
                        var allProductIds = _items.Select(i => i.ProductId).ToList();

                        checkoutActivity.PutExtra("Total", total.ToString());
                        checkoutActivity.PutExtra("ProductIds", JsonConvert.SerializeObject(allProductIds));

                        StartActivity(checkoutActivity);
                    }
                    else
                        Toast.MakeText(this, "No items in the shopping cart", ToastLength.Long);

                    break;
                case Resource.Id.action_home:
                    Intent homeActivity = new Intent(this, typeof(MainActivity));
                    StartActivity(homeActivity);
                    break;
                case Resource.Id.action_search:
                    Intent barcodeActivity = new Intent(this, typeof(ScanItemsActivity));
                    StartActivity(barcodeActivity);
                    break;
                case Resource.Id.action_login:
                    Intent login = new Intent(this, typeof(LoginActivity));
                    StartActivity(login);
                    break;
                case Resource.Id.action_logoff:
                    Logoff();
                    break;
                case Resource.Id.action_remove:
                    RemoveShoppingCartItem();
                    break;
                default:
                    break;
            }

            return true;
        }

        private void Logoff()
        {
            Helper.MobileService.LogoutAsync();
            //Remove shared values
            ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.Clear();
            editor.Commit();

            //Go to home page so it can enable the icons
            Intent homeActivity = new Intent(this, typeof(MainActivity));
            StartActivity(homeActivity);
        }

        private async void RemoveShoppingCartItem()
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                await Helper.MobileService.InvokeApiAsync($"v1/shopping-cart/{ShoppingCartId}", HttpMethod.Delete, null);
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

        protected override async void OnCreate(Bundle bundle)
        {
            
            base.OnCreate(bundle);

            ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
            string customerId = preferences.GetString("CustomerId", null);

            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                var list = await
                            Helper.MobileService.InvokeApiAsync($"v1/shopping-cart/customer/{customerId}", HttpMethod.Get, null);

                List<ShoppingCartModel> modelList = list.ToObject<List<ShoppingCartModel>>();

                _items = new List<CommonItem>();

                foreach (var item in modelList)
                {
                    _items.Add(new CommonItem()
                    {
                        Heading = $"Product: {item.ProductName}",
                        SubHeading = item.PriceIncTax.ToString(),
                        ImageResourceId = item.ShoppingCartId,
                        PriceIncTax = item.PriceIncTax,
                        ProductId = item.ProductId
                    });
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
            if (_items != null && _items.Count > 0)
            {
                var listView = sender as ListView;
                var t = _items[e.Position];

                ShoppingCartId = t.ImageResourceId; 

                Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}