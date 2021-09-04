using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Hooks
{
    using Drivers;
    using TechTalk.SpecFlow;

    [Binding]
    public class AppiumHooks
    {
        private readonly AppiumDriver _appiumDriver;

        public AppiumHooks(AppiumDriver appiumDriver)
        {
            _appiumDriver = appiumDriver;
        }

        [BeforeScenario()]
        public void StartApp()
        {
            _appiumDriver.StartApp();
        }

        [AfterScenario()]
        public void ShutdownApp()
        {
            _appiumDriver.StopApp();
        }
    }
}
