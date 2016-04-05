using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = true, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class MainActivity : CommonActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.Main);
        }
    }
}

