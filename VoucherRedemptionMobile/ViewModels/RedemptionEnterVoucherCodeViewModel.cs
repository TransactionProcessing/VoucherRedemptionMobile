namespace VoucherRedemptionMobile.ViewModels
{
    using System;
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.BindableObject" />
    public class RedemptionEnterVoucherCodeViewModel : BindableObject
    {
        #region Fields

        /// <summary>
        /// The voucher code
        /// </summary>
        private String voucherCode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the voucher code.
        /// </summary>
        /// <value>
        /// The voucher code.
        /// </value>
        public String VoucherCode
        {
            get
            {
                return this.voucherCode;
            }
            set
            {
                this.voucherCode = value;
                this.OnPropertyChanged(nameof(this.VoucherCode));
            }
        }

        #endregion
    }
}