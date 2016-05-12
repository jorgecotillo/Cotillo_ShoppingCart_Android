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

namespace Cotillo_ShoppingCart_Android.TableItems
{
    public class CommonItem
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public int ImageResourceId { get; set; }
        public double PriceIncTax { get; set; }
        public int ProductId { get; set; }
    }
}