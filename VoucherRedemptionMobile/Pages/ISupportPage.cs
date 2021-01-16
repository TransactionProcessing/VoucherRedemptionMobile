namespace VoucherRedemptionMobile.Pages
{
    using System;

    public interface ISupportPage
    {
        #region Events

        /// <summary>
        /// Occurs when [upload logs button click].
        /// </summary>
        event EventHandler UploadLogsButtonClick;
        
        #endregion

        #region Methods

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        void Init();

        #endregion
    }
}