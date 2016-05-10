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
using System.Net.Http;
using Cotillo_ShoppingCart_Models;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class RegisterExternalActivity : CommonActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.RegisterExternal);

            TextView txtEmail = FindViewById<TextView>(Resource.Id.txtEmail);

            txtEmail.Click += TxtEmail_Click;
        }

        private async void TxtEmail_Click(object sender, EventArgs e)
        {
            //await MobileService.InvokeApiAsync<ExtendedUserInfoModel>("Account")
        }
    }
}