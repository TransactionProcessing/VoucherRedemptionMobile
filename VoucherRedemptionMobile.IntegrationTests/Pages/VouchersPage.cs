namespace VoucherRedemptionMobile.IntegrationTests.Pages
{
    using System;
    using Common;
    using Xamarin.UITest.Queries;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.IntegrationTests.Common.BasePage" />
    public class VouchersPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The voucher redemption button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> VoucherRedemptionButton;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VouchersPage"/> class.
        /// </summary>
        public VouchersPage()
        {
            this.VoucherRedemptionButton = x => x.Marked("VoucherRedemptionButton");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trait.
        /// </summary>
        /// <value>
        /// The trait.
        /// </value>
        protected override PlatformQuery Trait =>
            new PlatformQuery
            {
                Android = x => x.Marked("Vouchers"),
                iOS = x => x.Marked("Vouchers")
            };

        #endregion

        #region Methods

        /// <summary>
        /// Clicks the redeem voucher button.
        /// </summary>
        public void ClickRedeemVoucherButton()
        {
            AppManager.App.ScrollUpTo(this.VoucherRedemptionButton);
            this.app.WaitForElement(this.VoucherRedemptionButton);
            this.app.Tap(this.VoucherRedemptionButton);
        }

        #endregion
    }
}