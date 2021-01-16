namespace VoucherRedemptionMobile.Pages
{
    using System;
    using ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public interface IRedemptionVoucherDetailsPage
    {
        #region Events

        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        event EventHandler CancelButtonClick;

        /// <summary>
        /// Occurs when [redeem voucher button click].
        /// </summary>
        event EventHandler RedeemVoucherButtonClick;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void Init(RedemptionVoucherDetailsViewModel viewModel);

        #endregion
    }
}