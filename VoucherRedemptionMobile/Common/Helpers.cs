using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.Common
{
    using Xamarin.Essentials;

    public static class Helpers
    {
        public static void Vibrate(Int32 seconds)
        {
            try
            {
                // Or use specified time
                var duration = TimeSpan.FromSeconds(seconds);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Feature not supported on device
                // Do nothing here
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                // Do nothing here
            }
        }
    }
}
