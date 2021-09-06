namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System;
    using System.Threading.Tasks;
    using OpenQA.Selenium;

    public class TestModePage : BasePage
    {
        private readonly String PinEntry;
        private readonly String TestVoucherDataEntry;

        private readonly String SetTestModeButton;
        public TestModePage()
        {
            this.PinEntry = "PinEntry";
            this.TestVoucherDataEntry = "TestVoucherData";

            this.SetTestModeButton = "SetTestModeButton";
        }

        public async Task EnterPin(String pinNumber)
        {
            this.HideKeyboard();
            IWebElement element = await this.WaitForElementByAccessibilityId(this.PinEntry);
            element.SendKeys(pinNumber);
        }
        

        public async Task EnterTestVoucherData(String testVoucherData)
        {
            this.HideKeyboard();
            IWebElement element = await this.WaitForElementByAccessibilityId(this.TestVoucherDataEntry);
            element.SendKeys(testVoucherData);
        }

        public async Task ClickSetTestModeButton()
        {
            this.HideKeyboard();
            IWebElement element = await this.WaitForElementByAccessibilityId(this.SetTestModeButton);
            element.Click();
        }

        protected override String Trait => "TestModeLabel";
    }
}