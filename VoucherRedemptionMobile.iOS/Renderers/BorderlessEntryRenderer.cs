using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(VoucherRedemptionMobile.Controls.BorderlessEntry), typeof(VoucherRedemptionMobile.iOS.Renderers.BorderlessEntryRenderer))]

namespace VoucherRedemptionMobile.iOS.Renderers
{
    using Xamarin.Forms.Platform.iOS;

    public class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null)
            {
                this.Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}