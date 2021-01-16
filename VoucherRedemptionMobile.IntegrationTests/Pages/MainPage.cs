namespace VoucherRedemptionMobile.IntegrationTests.Pages
{
    using System;
    using Common;
    using Xamarin.UITest.Queries;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.IntegrationTests.Common.BasePage" />
    public class MainPage : BasePage
    {
        #region Fields

        /// <summary>
        /// The vouchers button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> VouchersButton;
        /// <summary>
        /// The reports button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> ReportsButton;
        /// <summary>
        /// The profile button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> ProfileButton;
        /// <summary>
        /// The support button
        /// </summary>
        private readonly Func<AppQuery, AppQuery> SupportButton;
        /// <summary>
        /// The available balance label
        /// </summary>
        private readonly Func<AppQuery, AppQuery> AvailableBalanceLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.VouchersButton = x => x.Marked("VouchersButton");
            this.ReportsButton = x => x.Marked("ReportsButton");
            this.ProfileButton = x => x.Marked("ProfileButton");
            this.SupportButton = x => x.Marked("SupportButton");
            this.AvailableBalanceLabel = x => x.Marked("AvailableBalanceValueLabel");
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
                Android = x => x.Marked("Main Menu"),
                iOS = x => x.Marked("Main Menu")
            };

        #endregion

        /// <summary>
        /// Clicks the vouchers button.
        /// </summary>
        public void ClickVouchersButton()
        {
            AppManager.App.ScrollUpTo(this.VouchersButton);
            this.app.WaitForElement(this.VouchersButton);
            this.app.Tap(this.VouchersButton);
        }

        /// <summary>
        /// Clicks the reports button.
        /// </summary>
        public void ClickReportsButton()
        {
            this.app.WaitForElement(this.ReportsButton);
            this.app.Tap(this.ReportsButton);
        }

        /// <summary>
        /// Clicks the profile button.
        /// </summary>
        public void ClickProfileButton()
        {
            this.app.WaitForElement(this.ProfileButton);
            this.app.Tap(this.ProfileButton);
        }

        /// <summary>
        /// Clicks the support button.
        /// </summary>
        public void ClickSupportButton()
        {
            this.app.WaitForElement(this.SupportButton);
            this.app.Tap(this.SupportButton);
        }
    }
}