namespace VoucherRedemptionMobile.Pages
{
    using System;

    public interface IRedemptionFailedPage
    {
        #region Events

        /// <summary>
        /// Occurs when [cancel button clicked].
        /// </summary>
        event EventHandler CancelButtonClicked;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init();

        #endregion
    }
}