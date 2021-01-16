namespace VoucherRedemptionMobile.IntegrationTests.Pages
{
    using System;
    using System.Linq;
    using Common;
    using Shouldly;
    using Xamarin.UITest.Queries;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.IntegrationTests.Common.BasePage" />
    public class VoucherDetailsPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The balance label
        /// </summary>
        private readonly Func<AppQuery, AppQuery> BalanceLabel;

        /// <summary>
        /// The expiry date label
        /// </summary>
        private readonly Func<AppQuery, AppQuery> ExpiryDateLabel;

        /// <summary>
        /// The redeem voucher button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> RedeemVoucherButton;

        /// <summary>
        /// The value label
        /// </summary>
        private readonly Func<AppQuery, AppQuery> ValueLabel;

        /// <summary>
        /// The voucher code label
        /// </summary>
        private readonly Func<AppQuery, AppQuery> VoucherCodeLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherDetailsPage"/> class.
        /// </summary>
        public VoucherDetailsPage()
        {
            this.VoucherCodeLabel = x => x.Marked("VoucherCodeLabel");
            this.ExpiryDateLabel = x => x.Marked("ExpiryDateLabel");
            this.ValueLabel = x => x.Marked("ValueLabel");
            this.BalanceLabel = x => x.Marked("BalanceLabel");
            this.RedeemVoucherButton = x => x.Marked("RedeemVoucherButton");
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
                Android = x => x.Marked("Voucher Details"),
                iOS = x => x.Marked("Voucher Details")
            };

        #endregion

        #region Methods

        /// <summary>
        /// Asserts the voucher details.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="value">The value.</param>
        /// <param name="balance">The balance.</param>
        /// <param name="expiryDate">The expiry date.</param>
        public void AssertVoucherDetails(String voucherCode,
                                         Decimal value,
                                         Decimal balance,
                                         DateTime expiryDate)
        {
            var voucherCodeLabel = this.app.WaitForElement(this.VoucherCodeLabel).SingleOrDefault();
            voucherCodeLabel.ShouldNotBeNull();
            voucherCodeLabel.Text.ShouldBe(voucherCode);

            var valueLabel = this.app.WaitForElement(this.ValueLabel).SingleOrDefault();
            valueLabel.ShouldNotBeNull();
            valueLabel.Text.ShouldBe(value.ToString());

            var balanceLabel = this.app.WaitForElement(this.BalanceLabel).SingleOrDefault();
            balanceLabel.ShouldNotBeNull();
            balanceLabel.Text.ShouldBe(balance.ToString());

            var expiryDateLabel = this.app.WaitForElement(this.ExpiryDateLabel).SingleOrDefault();
            expiryDateLabel.ShouldNotBeNull();
            expiryDateLabel.Text.ShouldBe(expiryDate.ToString());
        }

        /// <summary>
        /// Clicks the redeem voucher button.
        /// </summary>
        public void ClickRedeemVoucherButton()
        {
            AppManager.App.ScrollUpTo(this.RedeemVoucherButton);
            this.app.WaitForElement(this.RedeemVoucherButton);
            this.app.Tap(this.RedeemVoucherButton);
        }

        #endregion
    }
}