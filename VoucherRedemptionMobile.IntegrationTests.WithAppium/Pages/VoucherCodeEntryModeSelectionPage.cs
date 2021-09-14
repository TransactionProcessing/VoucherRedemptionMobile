using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System.Threading.Tasks;

    public class VoucherCodeEntryModeSelectionPage : BasePage
    {
        protected override String Trait => "VoucherCodeEntryModeSelectionLabel";

        private readonly String KeyEntryButton;

        private readonly String ScanButton;

        public VoucherCodeEntryModeSelectionPage()
        {
            this.KeyEntryButton = "KeyEntryButton";
            this.ScanButton = "ScanButton";
        }

        public async Task ClickKeyEntryButton()
        {
            var element = await this.WaitForElementByAccessibilityId(this.KeyEntryButton);
            element.Click();
        }

        public async Task ClickScanButton()
        {
            var element = await this.WaitForElementByAccessibilityId(this.ScanButton);
            element.Click();
        }
    }
}
