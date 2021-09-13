namespace VoucherRedemptionMobile.iOS
{
    using System;
    using System.Threading.Tasks;
    using UIKit;
    using VoucherRedemptionMobile.Common;
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IDevice" />
    public class iOSDevice : IDevice
    {
        #region Methods

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <returns></returns>
        public String GetDeviceIdentifier()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString().Replace("-", "");
        }

        /// <summary>
        /// Gets the software version.
        /// </summary>
        /// <returns></returns>
        public String GetSoftwareVersion()
        {
            // TODO:
            return String.Empty;
        }

        #endregion
    }
}