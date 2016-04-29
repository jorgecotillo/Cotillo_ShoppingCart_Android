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

namespace Cotillo_ShoppingCart_Android
{
    public class CommonListActivity : ListActivity
    {
        protected string Barcode { get; set; }
        protected static MobileServiceClient MobileService = new MobileServiceClient("https://cotilloshoppingcartazure20160410065220.azurewebsites.net/");

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.Actions, menu);
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
                case Resource.Id.action_add:
                    //Call Add to shopping cart service if Barcode is not null or empty
                    if (!String.IsNullOrEmpty(Barcode))
                    {
                        Toast
                            .MakeText(this, "Product added.", ToastLength.Short)
                        .Show();
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
    }
}