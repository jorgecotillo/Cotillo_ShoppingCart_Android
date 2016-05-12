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

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                TextView txtEmail = FindViewById<TextView>(Resource.Id.txtEnterEmail);
                TextView txtName = FindViewById<TextView>(Resource.Id.txtEnterName);

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    Toast.MakeText(this, "Enter an email please", ToastLength.Long);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    Toast.MakeText(this, "Enter a name please", ToastLength.Long);
                    return;
                }

                //Call Register API
                JToken token = JToken.FromObject(new RegisterExternalModel()
                {
                    Username = txtEmail.Text,
                    Name = txtName.Text,
                    ExternalAccount = ExternalAccount
                });

                var customerToken = 
                    await Helper.MobileService.InvokeApiAsync("v1/account/external", token);

                var customerModel = customerToken.ToObject<CustomerModel>();
                
                //Add the email value to shared preferences
                ISharedPreferences globalPreferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);
                ISharedPreferencesEditor editorDisplayName = globalPreferences.Edit();
                editorDisplayName.PutString("DisplayName", txtEmail.Text);
                editorDisplayName.PutString("CustomerId", customerModel.CustomerId.ToString());

                //Persit the changes
                editorDisplayName.Commit();

                //Upon successful registration, redirect to main activity
                Intent homeActivity = new Intent(this, typeof(MainActivity));
                StartActivity(homeActivity);
            }
            catch (Exception ex)
            {
                //Log error
                Toast.MakeText(this, "An error ocurred while processing your request", ToastLength.Long);
            }
            finally
            {
                progress.Dismiss();
            }
        }

        private async void TxtEmail_Click(object sender, EventArgs e)
        {
            //await MobileService.InvokeApiAsync<ExtendedUserInfoModel>("Account")
        }
    }
}