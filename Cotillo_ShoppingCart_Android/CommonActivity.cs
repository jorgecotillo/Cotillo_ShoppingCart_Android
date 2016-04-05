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

namespace Cotillo_ShoppingCart_Android
{
    public class CommonActivity : Activity
    {
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = this.MenuInflater;
            inflater.Inflate(Resource.Menu.Actions, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_home:
                    Intent homeActivity = new Intent(this, typeof(MainActivity));
                    StartActivity(homeActivity);
                    break;
                case Resource.Id.action_search:
                    Intent barcodeActivity = new Intent(this, typeof(ScanItems));
                    StartActivity(barcodeActivity);
                    break;
                case Resource.Id.action_add:
                    Toast.MakeText(this, "Add selected", ToastLength.Short)
                        .Show();
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}