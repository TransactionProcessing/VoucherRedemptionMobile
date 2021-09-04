namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Android;
    using OpenQA.Selenium.Appium.iOS;
    using Shouldly;

    public static class Extensions
    {
        // TODO: May need a platform switch
        public static AndroidElement GetAlert(this AndroidDriver<AndroidElement> driver)
        {
            return driver.FindElementByClassName("androidx.appcompat.widget.AppCompatTextView");
        }

        public static async Task<IWebElement> WaitForElementByAccessibilityId(this AndroidDriver<AndroidElement> driver, String selector, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(60);
            IWebElement element = null;
            await Retry.For(async () =>
                            {
                                element = driver.FindElementByAccessibilityId(selector);
                                element.ShouldNotBeNull();
                            });

            return element;

        }

        public static async Task<IWebElement> WaitForElementByAccessibilityId(this IOSDriver<IOSElement> driver, String selector, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(60);
            IWebElement element = null;
            await Retry.For(async () =>
                            {
                                element = driver.FindElementByAccessibilityId(selector);
                                element.ShouldNotBeNull();
                            });

            return element;

        }

        public static async Task WaitForNoElementByAccessibilityId(this IOSDriver<IOSElement> driver, String selector, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(60);

            await Retry.For(async () =>
                            {
                                var element = driver.FindElementByAccessibilityId(selector);
                                element.ShouldBeNull();
                            });

        }

        public static async Task WaitForNoElementByAccessibilityId(this AndroidDriver<AndroidElement> driver, String selector, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(60);

            await Retry.For(async () =>
                            {
                                var element = driver.FindElementByAccessibilityId(selector);
                                element.ShouldBeNull();
                            });

        }

        public static async Task WaitForToastMessage(this AndroidDriver<AndroidElement> driver,
                                                     String expectedToast)
        {
            await Retry.For(async () =>
                            {
                                var args = new Dictionary<string, object>
                                           {
                                               {"text", expectedToast},
                                               {"isRegexp", false}
                                           };
                                driver.ExecuteScript("mobile: isToastVisible", args);

                            });
        }

        public static async Task WaitForToastMessage(this IOSDriver<IOSElement> driver,
                                                     String expectedToast)
        {
            Boolean isDisplayed = false;
            int count = 0;
            do
            {
                if (driver.PageSource.Contains(expectedToast))
                {
                    Console.WriteLine(driver.PageSource);
                    isDisplayed = true;
                    break;
                }
                Thread.Sleep(200);//Add your custom wait if exists
                count++;

            } while (count < 10);

            Console.WriteLine(driver.PageSource);
            isDisplayed.ShouldBeTrue();
        }

        public static async Task<String> GetPageSource(this AndroidDriver<AndroidElement> driver)
        {
            return driver.PageSource;
        }

        public static async Task<String> GetPageSource(this IOSDriver<IOSElement> driver)
        {
            return driver.PageSource;
        }
    }
}