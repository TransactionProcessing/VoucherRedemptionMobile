using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System.Threading.Tasks;
    using OpenQA.Selenium;

    public class MainPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The vouchers button
        /// </summary>
        private readonly String VouchersButton;
        /// <summary>
        /// The reports button
        /// </summary>
        private readonly String ReportsButton;
        /// <summary>
        /// The profile button
        /// </summary>
        private readonly String ProfileButton;
        /// <summary>
        /// The support button
        /// </summary>
        private readonly String SupportButton;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.VouchersButton = "VouchersButton";
            this.ReportsButton = "ReportsButton";
            this.ProfileButton = "ProfileButton";
            this.SupportButton = "SupportButton";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trait.
        /// </summary>
        /// <value>
        /// The trait.
        /// </value>
        protected override String Trait => "HomeLabel";

        #endregion

        /// <summary>
        /// Clicks the vouchers button.
        /// </summary>
        public async Task ClickVouchersButton()
        {
            IWebElement element = await this.WaitForElementByAccessibilityId(this.VouchersButton);
            element.Click();
        }

        /// <summary>
        /// Clicks the reports button.
        /// </summary>
        public async Task ClickReportsButton()
        {
            IWebElement element = await this.WaitForElementByAccessibilityId(this.ReportsButton);
            element.Click();
        }

        /// <summary>
        /// Clicks the profile button.
        /// </summary>
        public async Task ClickProfileButton()
        {
            IWebElement element = await this.WaitForElementByAccessibilityId(this.ProfileButton);
            element.Click();
        }

        /// <summary>
        /// Clicks the support button.
        /// </summary>
        public async Task ClickSupportButton()
        {
            IWebElement element = await this.WaitForElementByAccessibilityId(this.SupportButton);
            element.Click();
        }
    }
}
