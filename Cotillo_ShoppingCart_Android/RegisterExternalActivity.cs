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
using Newtonsoft.Json.Linq;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class RegisterExternalActivity : Activity
    {
        protected static MobileServiceClient MobileService = new MobileServiceClient("https://cotilloshoppingcartazure20160410065220.azurewebsites.net/");
        private string ExternalAccount
        {
            get; set;
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.RegisterExternal);

            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            btnRegister.Click += BtnRegister_Click;

            string externalAccount = Intent.GetStringExtra("ExternalAccount");
            this.ExternalAccount = externalAccount;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                TextView txtEmail = FindViewById<TextView>(Resource.Id.txtEnterEmail);
                if (String.IsNullOrWhiteSpace(txtEmail.Text))
                    Toast.MakeText(this, "Enter an email please", ToastLength.Long);

                //Call Register API
                JToken token = JToken.FromObject(new RegisterExternalModel()
                {
                    Username = txtEmail.Text,
                    ExternalAccount = ExternalAccount
                });
                MobileService.InvokeApiAsync("v1/account/external", token);
                
                //Add the email value to shared preferences
                ISharedPreferences globalPreferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
                ISharedPreferencesEditor editorDisplayName = globalPreferences.Edit();
                editorDisplayName.PutString("DisplayName", txtEmail.Text);

                //Persit the changes
                editorDisplayName.Commit();

                //Upon successful registration, redirect to main activity
                Intent homeActivity = new Intent(this, typeof(MainActivity));
                StartActivity(homeActivity);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private async void TxtEmail_Click(object sender, EventArgs e)
        {
            //await MobileService.InvokeApiAsync<ExtendedUserInfoModel>("Account")
        }
    }
}