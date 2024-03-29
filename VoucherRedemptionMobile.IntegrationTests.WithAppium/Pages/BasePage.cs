﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Pages
{
    using System.Threading.Tasks;
    using Common;
    using Features;
    using OpenQA.Selenium;
    using Shouldly;
    using VoucherRedemptionMobile.IntegrationTests.WithAppium.Drivers;

    public abstract class BasePage
    {
        protected abstract String Trait { get; }

        public async Task AssertOnPage(TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(60);

            await Retry.For(async () =>
            {
                String message = "Unable to verify on page: " + this.GetType().Name + $"Source [{await this.GetPageSource()}]";

                Should.NotThrow(() => this.WaitForElementByAccessibilityId(this.Trait), message);
            },
                            TimeSpan.FromMinutes(5),
                            timeout).ConfigureAwait(false);

        }

        /// <summary>
        /// Verifies that the trait is no longer present. Defaults to a 5 second wait.
        /// </summary>
        /// <param name="timeout">Time to wait before the assertion fails</param>
        public void WaitForPageToLeave(TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var message = "Unable to verify *not* on page: " + this.GetType().Name;

            Should.NotThrow(() => this.WaitForNoElementByAccessibilityId(this.Trait), message);
        }

        public async Task<IWebElement> WaitForElementByAccessibilityId(String x, TimeSpan? timeout = null)
        {
            try
            {
                if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
                {
                    return await AppiumDriver.AndroidDriver.WaitForElementByAccessibilityId(x, timeout);
                }
                else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
                {
                    return await AppiumDriver.iOSDriver.WaitForElementByAccessibilityId(x, timeout);
                }

                return null;
            }
            catch(Exception ex)
            {
                throw new Exception($"Element {x} not found on page");
            }
        }

        public async Task<String> GetPageSource()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                return await AppiumDriver.AndroidDriver.GetPageSource();
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                return await AppiumDriver.iOSDriver.GetPageSource();
            }

            return null;
        }

        public async Task WaitForNoElementByAccessibilityId(String x)
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                await AppiumDriver.AndroidDriver.WaitForNoElementByAccessibilityId(x);
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                await AppiumDriver.iOSDriver.WaitForNoElementByAccessibilityId(x);
            }
        }

        public async Task WaitForToastMessage(String x)
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                await AppiumDriver.AndroidDriver.WaitForToastMessage(x);
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                await AppiumDriver.iOSDriver.WaitForToastMessage(x);
            }
        }

        public void HideKeyboard()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                AppiumDriver.AndroidDriver.HideKeyboard();
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                //if (AppiumDriver.iOSDriver.IsKeyboardShown())
                //    AppiumDriver.iOSDriver.HideKeyboard();
                //AppiumDriver.iOSDriver.FindElementByName("Done").Click();
                //AppiumDriver.iOSDriver.HideKeyboard();
            }
        }

        public IWebElement GetAlert()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                return AppiumDriver.AndroidDriver.FindElementByClassName("androidx.appcompat.widget.AppCompatTextView");
            }

            return null;
        }

        public IAlert SwitchToAlert()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                return AppiumDriver.AndroidDriver.SwitchTo().Alert();
            }

            return null;
        }

        public void NavigateBack()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                AppiumDriver.AndroidDriver.Navigate().Back();
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                AppiumDriver.iOSDriver.Navigate().Back();
            }
        }
    }
}
