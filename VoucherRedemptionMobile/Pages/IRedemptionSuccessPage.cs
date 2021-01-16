namespace VoucherRedemptionMobile.Pages
{
    using System;

    public interface IRedemptionSuccessPage
    {
        #region Events

        /// <summary>
        /// Occurs when [complete button clicked].
        /// </summary>
        event EventHandler CompleteButtonClicked;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init();

        #endregion
    }
}