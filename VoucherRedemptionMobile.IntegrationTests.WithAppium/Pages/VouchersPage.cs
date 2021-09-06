namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System;
    using System.Threading.Tasks;

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
        private readonly String VoucherRedemptionButton;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VouchersPage"/> class.
        /// </summary>
        public VouchersPage()
        {
            this.VoucherRedemptionButton ="VoucherRedemptionButton";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trait.
        /// </summary>
        /// <value>
        /// The trait.
        /// </value>
        protected override String Trait => "VoucherLabel";

        #endregion

        #region Methods

        /// <summary>
        /// Clicks the redeem voucher button.
        /// </summary>
        public async Task ClickRedeemVoucherButton()
        {
            var element = await this.WaitForElementByAccessibilityId(this.VoucherRedemptionButton);
            element.Click();
        }

        #endregion
    }
}