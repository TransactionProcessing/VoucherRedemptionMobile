namespace VoucherRedemptionMobile.Droid
{
    using System;
    using Android.Content;
    using Android.OS;
    using Android.Provider;
    using Android.Util;
    using VoucherRedemptionMobile.Common;
    using Xamarin.Forms;
    using Application = Android.App.Application;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IDevice" />
    public class AndroidDevice : IDevice
    {
        #region Methods
        
        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <returns></returns>
        public String GetDeviceIdentifier()
        {
            String id = Build.Serial;
            if (string.IsNullOrWhiteSpace(id) || id == Build.Unknown || id == "0")
            {
                try
                {
                    Context context = Application.Context;
                    id = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
                }
                catch(Exception ex)
                {
                    Log.Warn("DeviceInfo", "Unable to get id: " + ex);
                }
            }

            return id;
        }

        /// <summary>
        /// Gets the software version.
        /// </summary>
        /// <returns></returns>
        public String GetSoftwareVersion()
        {
            String softwareVersion = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0).VersionName;

            return softwareVersion;
        }

        #endregion
    }
}