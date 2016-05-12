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

namespace Cotillo_ShoppingCart_Android
{
    public static class Helper
    {
        public static MobileServiceClient MobileService =
            new MobileServiceClient("https://cotilloshoppingcartazure20160410065220.azurewebsites.net/");
    }
}