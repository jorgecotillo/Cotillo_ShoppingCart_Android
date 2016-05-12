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
using Newtonsoft.Json.Linq;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class ScanItemsActivity : CommonActivity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScanItem);

            Button btnAddToCart = FindViewById<Button>(Resource.Id.btnAddToCart);
            btnAddToCart.Click += BtnAddToCart_Click;

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
            //Display progress dialog so the user knows that there is an event taking action

            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                //Call Azure App Service (Web Api) to get Product Info
                var product =
                    await Helper.MobileService.InvokeApiAsync<ProductModel>($"v1/products/barcode/{scanResult.Text}", HttpMethod.Get, null);

                //Set ProductId in CommonActivity, this value will be used later in case the product
                //is added to the shopping cart
                base.ProductId = product.Id;

                //Set Price, value to be used if the item is added to the cart
                base.PriceIncTax = product.PriceIncTax;

                //Display product info
                var productTitle = FindViewById<TextView>(Resource.Id.product_title);
                productTitle.Visibility = ViewStates.Visible;
                productTitle.Text = product.Name;

                var productDescriptionLabel = FindViewById<TextView>(Resource.Id.product_description_label);
                productDescriptionLabel.Visibility = ViewStates.Visible;

                var productDescription = FindViewById<TextView>(Resource.Id.product_description);
                productDescription.Visibility = ViewStates.Visible;
                productDescription.Text = product.Description;

                var productExpirationDateLabel = FindViewById<TextView>(Resource.Id.product_expiration_date_label);
                productExpirationDateLabel.Visibility = ViewStates.Visible;

                var productExpirationDate = FindViewById<TextView>(Resource.Id.product_expiration_date);
                productExpirationDate.Visibility = ViewStates.Visible;
                productExpirationDate.Text = product.ExpiresOn;

                var productIncTaxLabel = FindViewById<TextView>(Resource.Id.product_price_incl_tax_label);
                productIncTaxLabel.Visibility = ViewStates.Visible;

                var productIncTax = FindViewById<TextView>(Resource.Id.product_price_incl_tax);
                productIncTax.Visibility = ViewStates.Visible;
                productIncTax.Text = product.PriceIncTax.ToString("C");

                var image = FindViewById<ImageView>(Resource.Id.imageView1);
                image.Visibility = ViewStates.Visible;
                Bitmap bMap = BitmapFactory.DecodeByteArray(product.Image, 0, product.Image.Length);
                image.SetImageBitmap(bMap);

                Button btnAddToCart = FindViewById<Button>(Resource.Id.btnAddToCart);
                btnAddToCart.Visibility = ViewStates.Visible;

                //Setting the barcode property, this value is required so that the menu item (Add to shopping cart) can work.
                //Add to shopping cart menu item looks up at this property and uses it to map to the proper product (probably good to use SharedPreferences instead?)
                base.Barcode = scanResult.Text;
            }
            catch(Exception ex)
            {
                //Log error
                Toast.MakeText(this, "An error ocurred while processing your request", ToastLength.Long);
            }
            finally
            {
                //Remove progress bar, page is fully loaded
                progress.Dismiss();
            }
        }

        private async void BtnAddToCart_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
                string customerId = preferences.GetString("CustomerId", null);

                List<ShoppingCartModel> shoppingCartItems = new List<ShoppingCartModel>();

                shoppingCartItems.Add(new ShoppingCartModel()
                {
                    ProductId = ProductId,
                    Quantity = 1,
                    PriceIncTax = PriceIncTax
                });

                var token = JToken.FromObject(shoppingCartItems);

                await Helper.MobileService.InvokeApiAsync($"v1/shopping-cart/customer/{customerId}", token);

                Toast.MakeText(this, "Product successfully added, select scan or go to home page to continue", ToastLength.Long);

                Button btnAddToCart = FindViewById<Button>(Resource.Id.btnAddToCart);
                btnAddToCart.Enabled = false;
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