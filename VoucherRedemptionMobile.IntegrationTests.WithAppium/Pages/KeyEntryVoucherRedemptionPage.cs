namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System;
    using System.Threading.Tasks;
    using OpenQA.Selenium;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="BasePage" />
    public class KeyEntryVoucherRedemptionPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The find voucher button
        /// </summary>
        private readonly String FindVoucherButton;

        /// <summary>
        /// The voucher code entry
        /// </summary>
        private readonly String VoucherCodeEntry;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEntryVoucherRedemptionPage"/> class.
        /// </summary>
        public KeyEntryVoucherRedemptionPage()
        {
            this.VoucherCodeEntry = "VoucherCodeEntry";
            this.FindVoucherButton = "FindVoucherButton";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trait.
        /// </summary>
        /// <value>
        /// The trait.
        /// </value>
        protected override String Trait => "EnterVoucherCodeLabel";
            
        #endregion

        #region Methods

        /// <summary>
        /// Clicks the find voucher button.
        /// </summary>
        public async Task ClickFindVoucherButton()
        {
            var element = await this.WaitForElementByAccessibilityId(this.FindVoucherButton);
            element.Click();
        }

        /// <summary>
        /// Enters the voucher code.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        public async Task EnterVoucherCode(String voucherCode)
        {
            IWebElement element = await this.WaitForElementByAccessibilityId(this.VoucherCodeEntry);

            element.SendKeys(voucherCode);
        }

        #endregion
    }
}