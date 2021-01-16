namespace VoucherRedemptionMobile.Common
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IDevice
    {
        #region Methods

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        /// <returns></returns>
        String GetDeviceIdentifier();

        /// <summary>
        /// Gets the software version.
        /// </summary>
        /// <returns></returns>
        String GetSoftwareVersion();

        #endregion
    }
}