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
using ZXing.Mobile;
using System.Threading.Tasks;
using System.Net.Http;
using Cotillo_ShoppingCart_Models;
using Android.Graphics;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class ScanItemsActivity : CommonActivity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScanItem);

            MobileBarcodeScanner.Initialize(Application);

            var result = await ScanItem();

            if (result != null)
            {
                await GetProductInfo(result);
            }
            else
            {
                //This else is hit when the user presses back button on the device
                //When the code hits the else, we will redirect the user to the home page, otherwise 
                //by clicking back, since the scanning creates a new Layout, the user will get redirected
                //to ScanItem.axml with no data
                Intent homeActivity = new Intent(this, typeof(MainActivity));
                StartActivity(homeActivity);
            }
        }

        private async Task<ZXing.Result> ScanItem()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            return result;
        }

        private async Task GetProductInfo(ZXing.Result scanResult)
        {
            //Call Azure App Service (Web Api) to get Product Info
            var product = 
                await MobileService.InvokeApiAsync<ProductModel>($"v1/products/barcode/{scanResult.Text}", HttpMethod.Get, null);

            //Display product info
            var productTitle = FindViewById<TextView>(Resource.Id.product_title);
            productTitle.Visibility = ViewStates.Visible;
            productTitle.Text = product.Name;

            var productDescription = FindViewById<TextView>(Resource.Id.product_description);
            productDescription.Visibility = ViewStates.Visible;
            productDescription.Text = product.Name;

            var image = FindViewById<ImageView>(Resource.Id.imageView1);
            Bitmap bMap = BitmapFactory.DecodeByteArray(product.Image, 0, product.Image.Length);
            image.SetImageBitmap(bMap);
        }
    }
}