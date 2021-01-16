using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.Controls
{
    public interface IVoucherCodeInput
    {
        #region Properties

        /// <summary>
        /// Gets or sets the code entry value.
        /// </summary>
        /// <value>
        /// The code entry value.
        /// </value>
        String CodeEntryValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the input.
        /// </summary>
        /// <value>
        /// The maximum length of the input.
        /// </value>
        Int32 MaximumInputLength { get; set; }

        #endregion

        #region Methods

        void DisableScanning();

        void EnableScanning();

        /// <summary>
        /// Gives the code entry focus.
        /// </summary>
        void GiveCodeEntryFocus();

        /// <summary>
        /// Sets the description text.
        /// </summary>
        /// <param name="text">The text.</param>
        void SetDescriptionText(String text);

        /// <summary>
        /// Sets the header text.
        /// </summary>
        /// <param name="text">The text.</param>
        void SetHeaderText(String text);

        /// <summary>
        /// Sets the message text.
        /// </summary>
        /// <param name="text">The text.</param>
        void SetMessageText(String text);

        #endregion

        void ClearMessageText();
    }
}
