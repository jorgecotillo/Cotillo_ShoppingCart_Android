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
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using Cotillo_ShoppingCart_Models;
using Newtonsoft.Json;

namespace Cotillo_ShoppingCart_Android
{
    [Activity(Label = "Cotillo's SuperMarket", MainLauncher = false, Icon = "@drawable/ic_shopping_cart_white_24dp")]
    public class LoginActivity : CommonActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            ImageView facebookLogin = FindViewById<ImageView>(Resource.Id.facebook_image);
            facebookLogin.Click += FacebookLogin_Click;

        }

        private async void FacebookLogin_Click(object sender, EventArgs e)
        {
            await AuthenticateUserCachedTokenAsync(MobileServiceAuthenticationProvider.Facebook);
            //var x = await MobileService.InvokeApiAsync("Values", HttpMethod.Get, null);
        }

        private static string USERID_PREFID = "userId:";
        private static string AUTHTOKEN_PREFID = "authToken:";

        /// <summary>
        /// Authenticate user as an asynchronous operation caching the authorization on client token
        /// </summary>
        /// <param name="providerType">Type of the provider.</param>
        /// <returns>Task.</returns>
        public async Task AuthenticateUserCachedTokenAsync(MobileServiceAuthenticationProvider providerType = MobileServiceAuthenticationProvider.Twitter)
        {
            ISharedPreferences preferences = this.GetPreferences(FileCreationMode.Private);

            // Try to get an existing credential from the preferences.

            string userId = preferences.GetString(USERID_PREFID + providerType, null);
            string authToken = preferences.GetString(AUTHTOKEN_PREFID + providerType, null);

            // There are credentials in the private preferences, get it from the preferences, verify that it's still valid
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(authToken))
            {
                MobileService.CurrentUser = await GetUserFromPreferences(userId, providerType, authToken, preferences);
            }
            
            
            // If we have a user then we are done, if not then prompt the user to login
            // and save the credentials in the preferences
            while (MobileService.CurrentUser == null || MobileService.CurrentUser.UserId == null)
            {
                string message;

                try
                {
                    // Authenticate using provided provider type.
                    MobileServiceUser mobileServiceUser = await MobileService.LoginAsync(this, providerType);

                    // Create the credential package to store in the preferences
                    ISharedPreferencesEditor editor = preferences.Edit();
                    editor.PutString(USERID_PREFID + providerType, mobileServiceUser.UserId);
                    editor.PutString(AUTHTOKEN_PREFID + providerType, mobileServiceUser.MobileServiceAuthenticationToken);

                    // Persist the credential package to the preferences
                    editor.Commit();
                }
                catch (InvalidOperationException ex)
                {
                    message = "You must log in. Login Required" + ex.Message;
                }
            }

            //All is good, get Display Data
            ISharedPreferences globalPreferences = this.GetSharedPreferences("globalValues", FileCreationMode.Private);

            string displayName = globalPreferences.GetString("DisplayName", null);

            if (String.IsNullOrWhiteSpace(displayName))
            {
                try
                {
                    string filteredUserId = MobileService.CurrentUser.UserId.Replace("sid:", string.Empty);
                    using (var client = new System.Net.Http.HttpClient())
                    {   
                        // Request the current user info from Facebook.
                        using (var resp = await client.GetAsync($"https://cotilloshoppingcartazure20160410065220.azurewebsites.net/api/v1/account/external-user/{filteredUserId}"))
                        {
                            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                            {
                                //Display Register
                                Intent registerExternalActivity = new Intent(this, typeof(RegisterExternalActivity));
                                registerExternalActivity.PutExtra("ExternalAccount", MobileService.CurrentUser.UserId);
                                StartActivity(registerExternalActivity);
                                return;
                            }
                            else
                            {
                                var userInfo = await resp.Content.ReadAsStringAsync();

                                ExtendedUserInfoModel extendedUserInfo = 
                                    JsonConvert.DeserializeObject<ExtendedUserInfoModel>(userInfo);

                                if (extendedUserInfo != null)
                                {
                                    // Create the display package to store the display name
                                    ISharedPreferencesEditor editorDisplayName = globalPreferences.Edit();
                                    editorDisplayName.PutString("DisplayName", extendedUserInfo.Name);

                                    //Persit the changes
                                    editorDisplayName.Commit();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Toast
                        .MakeText(this, "An error ocurred while processing your request, please try in a few minutes", ToastLength.Long)
                        .Show();
                }
            }

            //Now redirect to Main Activity and show user in title bar
            Intent homeActivity = new Intent(this, typeof(MainActivity));
            StartActivity(homeActivity);
        }

        /// <summary>
        /// Gets the user from preferences.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="providerType">Type of the provider.</param>
        /// <param name="authToken">The authentication token.</param>
        /// <param name="preferences">The preferences.</param>
        /// <returns>Task&lt;MobileServiceUser&gt;.</returns>
        private async Task<MobileServiceUser> GetUserFromPreferences(string userId, MobileServiceAuthenticationProvider providerType, string authToken, ISharedPreferences preferences)
        {
            // Create a user from the stored credentials.
            MobileServiceUser mobileServiceUser = new MobileServiceUser(userId);
            mobileServiceUser.MobileServiceAuthenticationToken = authToken;

            // Set the user from the stored credentials.
            MobileService.CurrentUser = mobileServiceUser;

            try
            {
                // Try to make a call to verify that the credential has not expired
                await MobileService.InvokeApiAsync("Alive", HttpMethod.Get, null);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                if (ex.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ISharedPreferencesEditor editor = preferences.Edit();

                    // Remove the credentials with the expired token.
                    editor.Remove(USERID_PREFID + providerType);
                    editor.Remove(AUTHTOKEN_PREFID + providerType);
                    editor.Commit();

                    mobileServiceUser = null;
                }
            }

            return mobileServiceUser;
        }
    }
}