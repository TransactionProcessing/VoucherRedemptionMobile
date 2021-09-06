namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BasePage" />
    public class VoucherDetailsPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The balance label
        /// </summary>
        private readonly String BalanceLabel;

        /// <summary>
        /// The expiry date label
        /// </summary>
        private readonly String ExpiryDateLabel;

        /// <summary>
        /// The redeem voucher button
        /// </summary>
        private readonly String RedeemVoucherButton;

        /// <summary>
        /// The value label
        /// </summary>
        private readonly String ValueLabel;

        /// <summary>
        /// The voucher code label
        /// </summary>
        private readonly String VoucherCodeLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherDetailsPage"/> class.
        /// </summary>
        public VoucherDetailsPage()
        {
            this.VoucherCodeLabel = "VoucherCodeLabel";
            this.ExpiryDateLabel = "ExpiryDateLabel";
            this.ValueLabel = "ValueLabel";
            this.BalanceLabel = "BalanceLabel";
            this.RedeemVoucherButton = "RedeemVoucherButton";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trait.
        /// </summary>
        /// <value>
        /// The trait.
        /// </value>
        protected override String Trait => "VoucherDetailsLabel";

        #endregion

        #region Methods

        /// <summary>
        /// Asserts the voucher details.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="value">The value.</param>
        /// <param name="balance">The balance.</param>
        /// <param name="expiryDate">The expiry date.</param>
        public async Task AssertVoucherDetails(String voucherCode,
                                         Decimal value,
                                         Decimal balance,
                                         DateTime expiryDate)
        {
            var voucherCodeLabel = await this.WaitForElementByAccessibilityId(this.VoucherCodeLabel);
            voucherCodeLabel.ShouldNotBeNull();
            voucherCodeLabel.Text.ShouldBe(voucherCode);

            var valueLabel = await this.WaitForElementByAccessibilityId(this.ValueLabel);
            valueLabel.ShouldNotBeNull();
            valueLabel.Text.ShouldBe(value.ToString());

            var balanceLabel = await this.WaitForElementByAccessibilityId(this.BalanceLabel);
            balanceLabel.ShouldNotBeNull();
            balanceLabel.Text.ShouldBe(balance.ToString());

            var expiryDateLabel = await this.WaitForElementByAccessibilityId(this.ExpiryDateLabel);
            expiryDateLabel.ShouldNotBeNull();
            //expiryDateLabel.Text.ShouldBe(expiryDate.ToString());
            var actualExpiryDate = DateTime.Parse(expiryDateLabel.Text);
            actualExpiryDate.ShouldBe(expiryDate, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Clicks the redeem voucher button.
        /// </summary>
        public async Task ClickRedeemVoucherButton()
        {
            var element = await this.WaitForElementByAccessibilityId(this.RedeemVoucherButton);
            element.Click();
        }

        #endregion
    }
}