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
using Newtonsoft.Json.Linq;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = true, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class RegularLoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.RegularLogin);

            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnLogin.Click += BtnLogin_Click;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.SetTitle("Loading");
            progress.SetMessage("Wait while loading...");
            progress.Show();

            try
            {
                var username = FindViewById<TextView>(Resource.Id.txtUser);
                var password = FindViewById<TextView>(Resource.Id.txtPassword);

                LoginModel model = new LoginModel()
                {
                    UserName = username.Text,
                    Password = password.Text
                };

                var result = await Helper.MobileService.InvokeApiAsync("v1/account/login", JToken.FromObject(model));

                var extendedUserInfo = result.ToObject<ExtendedUserInfoModel>();

                ISharedPreferences globalPreferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);

                // Create the display package to store the display name
                ISharedPreferencesEditor editorDisplayName = globalPreferences.Edit();
                editorDisplayName.PutString("DisplayName", extendedUserInfo.Name);
                editorDisplayName.PutString("CustomerId", extendedUserInfo.UserId);
                //Persit the changes
                editorDisplayName.Commit();
            }
            catch (Exception ex)
            {
                //Log error
                Toast.MakeText(this, "An error occurred while processing your request", ToastLength.Long);
            }
            finally
            {
                progress.Dismiss();
                Intent homeActivity = new Intent(this, typeof(MainActivity));
                StartActivity(homeActivity);
            }
        }
    }
}