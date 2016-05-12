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
using Microsoft.WindowsAzure.MobileServices;
using Cotillo_ShoppingCart_Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Cotillo_ShoppingCart_Android
{
    public class CommonActivity : Activity
    {
        protected string Barcode { get; set; }
        protected int ProductId { get; set; }
        protected static MobileServiceClient MobileService = new MobileServiceClient("https://cotilloshoppingcartazure20160410065220.azurewebsites.net/");

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //Inflate the items first so it can be manipulated later
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.Actions, menu);

            ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
            string displayName = preferences.GetString("DisplayName", null);

            if (!String.IsNullOrWhiteSpace(displayName))
            {
                var actionMenu = menu.FindItem(Resource.Id.action_login);
                actionMenu.SetVisible(false);

                var accountMenu = menu.FindItem(Resource.Id.action_account);
                accountMenu.SetVisible(true);
                accountMenu.SetShowAsAction(ShowAsAction.Always);

                var logoffMenu = menu.FindItem(Resource.Id.action_logoff);
                logoffMenu.SetVisible(true);
                logoffMenu.SetShowAsAction(ShowAsAction.Always);
            }
            else
            {
                var actionMenu = menu.FindItem(Resource.Id.action_login);
                actionMenu.SetVisible(true);

                var accountMenu = menu.FindItem(Resource.Id.action_account);
                accountMenu.SetVisible(false);

                var logoffMenu = menu.FindItem(Resource.Id.action_logoff);
                logoffMenu.SetVisible(false);
            }

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
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
                case Resource.Id.action_add:
                    //Call Add to shopping cart service if Barcode is not null or empty
                    if(!String.IsNullOrEmpty(Barcode))
                    {
                        AddToShoppingCart();

                        //Task
                        //    .Run(async () => await AddToShoppingCart())
                        //    .ContinueWith(i => Toast.MakeText(this, "Product added", ToastLength.Long))
                        //    .Wait();
                    }
                    else
                    {
                        Toast
                            .MakeText(this, "Please select a product first.", ToastLength.Short)
                        .Show();
                    }
                    
                    break;
                default:
                    break;
            }

            return true;
        }

        private void Logoff()
        {
            MobileService.LogoutAsync();
            //Remove shared values
            ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
            ISharedPreferencesEditor editor = preferences.Edit();
            editor.Clear();
            editor.Commit();

            //Go to home page so it can enable the icons
            Intent homeActivity = new Intent(this, typeof(MainActivity));
            StartActivity(homeActivity);
        }

        private async Task AddToShoppingCart()
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
                string customerId = preferences.GetString("CustomerId", null);
                
                var token = JToken.FromObject(new ShoppingCartModel()
                {
                    ProductId = ProductId,
                    Quantity = 1
                });

                var parameters = new Dictionary<string, string>();
                parameters.Add("customerId", customerId);

                await MobileService.InvokeApiAsync($"v1/shopping-cart/customer/{customerId}", token);

                Toast.MakeText(this, "Product successfully added, select scan or go to home page to continue", ToastLength.Long);
            }
            catch (Exception ex)
            {
                //Log the error
                Toast.MakeText(this, "An error ocurred while processing your request.", ToastLength.Long);
            }
            finally
            {
                progress.Dismiss();
            }
        }
    }
}