namespace VoucherRedemptionMobile.Pages
{
    using System;
    using ViewModels;
    using ZXing;

    /// <summary>
    /// 
    /// </summary>
    public interface IRedemptionEnterVoucherCodePage
    {
        #region Events

        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        event EventHandler CancelButtonClick;

        /// <summary>
        /// Occurs when [find voucher button click].
        /// </summary>
        event EventHandler FindVoucherButtonClick;

        #endregion

        #region Methods

        /// <summary>
        /// Clears the voucher code.
        /// </summary>
        void ClearVoucherCode();

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void Init(RedemptionEnterVoucherCodeViewModel viewModel);

        #endregion
    }

    public interface IRedemptionScanVoucherCodePage
    {
        void Init();

        event EventHandler<Result> VoucherBarcodeScanned;

        event EventHandler CancelButtonClick;
    }
}