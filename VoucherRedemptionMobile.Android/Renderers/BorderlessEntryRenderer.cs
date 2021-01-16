using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(VoucherRedemptionMobile.Controls.BorderlessEntry), typeof(VoucherRedemptionMobile.Droid.Renderers.BorderlessEntryRenderer))]

namespace VoucherRedemptionMobile.Droid.Renderers
{
    using Xamarin.Forms.Platform.Android;
    using Application = Android.App.Application;

    public class BorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessEntryRenderer() : base(Application.Context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null)
            {
                this.Control.SetBackground(null);
                Control.Gravity = GravityFlags.CenterVertical;
                Control.SetPadding(0, 0, 0, 0);
            }
        }
    }
}