namespace VoucherRedemptionMobile.IntegrationTests.Pages
{
    using System;
    using Common;
    using Xamarin.UITest.Queries;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.IntegrationTests.Common.BasePage" />
    public class VoucherRedemptionPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The find voucher button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> FindVoucherButton;

        /// <summary>
        /// The voucher code entry
        /// </summary>
        private readonly Func<AppQuery, AppQuery> VoucherCodeEntry;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherRedemptionPage"/> class.
        /// </summary>
        public VoucherRedemptionPage()
        {
            this.VoucherCodeEntry = x => x.Marked("VoucherCodeEntry");
            this.FindVoucherButton = x => x.Marked("FindVoucherButton");
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
                Android = x => x.Marked("Voucher Redemption"),
                iOS = x => x.Marked("Voucher Redemption")
            };

        #endregion

        #region Methods

        /// <summary>
        /// Clicks the find voucher button.
        /// </summary>
        public void ClickFindVoucherButton()
        {
            AppManager.App.ScrollUpTo(this.FindVoucherButton);
            this.app.WaitForElement(this.FindVoucherButton);
            this.app.Tap(this.FindVoucherButton);
        }

        /// <summary>
        /// Enters the voucher code.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        public void EnterVoucherCode(String voucherCode)
        {
            this.app.WaitForElement(this.VoucherCodeEntry);
            this.app.EnterText(this.VoucherCodeEntry, voucherCode);
        }

        #endregion
    }
}