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

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = true, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class ScanItems : CommonActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScanItem);

            MobileBarcodeScanner.Initialize(Application);

            Button buttonScan = FindViewById<Button>(Resource.Id.buttonScan);
            buttonScan.Click += async (sender, e) => 
            {

                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();

                if (result != null)
                    Console.WriteLine("Scanned Barcode: " + result.Text);
            };
        }
    }
}