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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class CheckoutActivity : Activity
    {
        private List<int> ProductIds { get; set; } 
        private double TotalIncTax { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Checkout);

            var total = FindViewById<TextView>(Resource.Id.txtTotal);

            string totalFromExtra = Intent.GetStringExtra("Total");
            TotalIncTax = double.Parse(totalFromExtra);

            total.Text = TotalIncTax.ToString("C");

            var productIds = Intent.GetStringExtra("ProductIds");
            ProductIds = JsonConvert.DeserializeObject<List<int>>(productIds);

            var btnProcess = FindViewById<Button>(Resource.Id.btnReview);
            btnProcess.Click += BtnProcess_Click;
        }

        private async void BtnProcess_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();
            var btnProcess = FindViewById<Button>(Resource.Id.btnReview);

            try
            {
                var total = FindViewById<TextView>(Resource.Id.txtTotal);

                CheckoutModel model = new CheckoutModel()
                {
                    TotalIncTax = TotalIncTax,
                    PaymentInfoModel = new PaymentInfoModel()
                    {
                        Address = "70 Blanchard",
                        City = "Burlington",
                        Country = "USA",
                        CreditCardNo = "123456",
                        CVV2 = 1820,
                        ExpireDate = DateTime.Now.AddYears(5)
                    },
                    ProductIds = ProductIds
                };

                ISharedPreferences preferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
                string customerId = preferences.GetString("CustomerId", null);

                JToken token = JToken.FromObject(model);

                await Helper.MobileService.InvokeApiAsync($"v1/orders/checkout/{customerId}", token);
            }
            catch (Exception ex)
            {
                //Log error
                Toast.MakeText(this, "An error occurred while processing your request", ToastLength.Long);
            }
            finally
            {
                progress.Dismiss();
                btnProcess.Enabled = false;
                Toast.MakeText(this, "Order successfully processed", ToastLength.Long);

                Intent homeActivity = new Intent(this, typeof(MainActivity));
                StartActivity(homeActivity);
            }
        }
    }
}