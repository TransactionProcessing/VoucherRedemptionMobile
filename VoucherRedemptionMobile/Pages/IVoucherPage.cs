using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.Pages
{
    public interface IVoucherPage
    {
        #region Methods

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        void Init();
        
        /// <summary>
        /// Occurs when [voucher redemption button click].
        /// </summary>
        event EventHandler VoucherRedemptionButtonClick;

        #endregion
    }
}
