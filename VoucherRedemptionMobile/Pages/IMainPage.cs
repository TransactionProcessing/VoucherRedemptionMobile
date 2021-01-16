namespace VoucherRedemptionMobile.Pages
{
    using System;
    using ViewModels;

    /// <summary>
    /// 
    /// </summary>
    public interface IMainPage
    {
        #region Events
        
        /// <summary>
        /// Occurs when [support button clicked].
        /// </summary>
        event EventHandler SupportButtonClicked;

        /// <summary>
        /// Occurs when [voucher button clicked].
        /// </summary>
        event EventHandler VoucherButtonClicked;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init(MainPageViewModel viewModel);

        #endregion
    }
}