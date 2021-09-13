using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.Pages
{
    public interface IRedemptionSelectVoucherEntryModePage
    {
        #region Events
        
        event EventHandler KeyEntryButtonClick;
        
        event EventHandler ScanButtonClick;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        void Init();

        #endregion
    }
}
