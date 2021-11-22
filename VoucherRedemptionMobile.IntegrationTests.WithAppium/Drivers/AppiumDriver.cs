using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using System;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Drivers
{
    using System.IO;
    using System.Reflection;
    using Features;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Enums;

    public class AppiumDriver
    {
        public static MobileTestPlatform MobileTestPlatform;

        public static AndroidDriver<AndroidElement> AndroidDriver;

        public static IOSDriver<IOSElement> iOSDriver;

        public AppiumDriver()
        {
        }

        public void StartApp()
        {
            AppiumLocalService appiumService = null;
            appiumService = new AppiumServiceBuilder().UsingPort(4723).Build();

            if (appiumService.IsRunning == false)
            {
                appiumService.Start();
            }

            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                var driverOptions = new AppiumOptions();
                driverOptions.AddAdditionalCapability("adbExecTimeout", TimeSpan.FromMinutes(5).Milliseconds);
                driverOptions.AddAdditionalCapability(MobileCapabilityType.AutomationName, "Espresso");
                // TODO: Only do this locally
                driverOptions.AddAdditionalCapability("forceEspressoRebuild", true);
                driverOptions.AddAdditionalCapability("enforceAppInstall", true);
                driverOptions.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "9.0");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.DeviceName, "emulator-5554");

                String assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                String binariesFolder = Path.Combine(assemblyFolder, "..", "..", "..", "..", @"VoucherRedemptionMobile.Android/bin/Release");
                var apkPath = Path.Combine(binariesFolder, "com.transactionprocessing.voucherredemptionmobile.apk");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.App, apkPath);
                driverOptions.AddAdditionalCapability("espressoBuildConfig",
                                                      "{ \"additionalAppDependencies\": [ \"com.google.android.material:material:1.0.0\", \"androidx.lifecycle:lifecycle-extensions:2.1.0\" ] }");

                AppiumDriver.AndroidDriver = new AndroidDriver<AndroidElement>(appiumService, driverOptions, TimeSpan.FromMinutes(5));
            }

            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                var driverOptions = new AppiumOptions();
                driverOptions.AddAdditionalCapability(MobileCapabilityType.PlatformName, "iOS");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.DeviceName, "iPhone 13");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "15.0");

                String assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                String binariesFolder = Path.Combine(assemblyFolder, "..", "..", "..", "..", @"VoucherRedemptionMobile.iOS/bin/iPhoneSimulator/Release");
                var apkPath = Path.Combine(binariesFolder, "VoucherRedemptionMobile.iOS.app");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.App, apkPath);
                //driverOptions.AddAdditionalCapability("bundleId", "com.companyname.VoucherRedemptionMobile");
                driverOptions.AddAdditionalCapability(MobileCapabilityType.FullReset, true);
                driverOptions.AddAdditionalCapability(MobileCapabilityType.AutomationName, "XCUITest");
                driverOptions.AddAdditionalCapability("useNewWDA", true);
                driverOptions.AddAdditionalCapability("wdaLaunchTimeout", 999999999);
                driverOptions.AddAdditionalCapability("wdaConnectionTimeout", 999999999);
                driverOptions.AddAdditionalCapability("restart", true);

                AppiumDriver.iOSDriver = new IOSDriver<IOSElement>(appiumService, driverOptions, TimeSpan.FromMinutes(5));
            }
        }
        
        public void StopApp()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                AppiumDriver.AndroidDriver.Quit();
            }
            else if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                AppiumDriver.iOSDriver.Quit();
            }
        }
    }
}
