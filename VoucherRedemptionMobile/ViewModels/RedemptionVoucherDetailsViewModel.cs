namespace VoucherRedemptionMobile.ViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class RedemptionVoucherDetailsViewModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>
        /// The expiry date.
        /// </value>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public Decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the voucher code.
        /// </summary>
        /// <value>
        /// The voucher code.
        /// </value>
        public String VoucherCode { get; set; }

        #endregion
    }
}