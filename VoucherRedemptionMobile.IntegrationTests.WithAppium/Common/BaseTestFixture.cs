using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Features
{
    using Drivers;
    using NUnit.Framework;

    public enum MobileTestPlatform
    {
        //Android,

        iOS,
        Android
    }

    public abstract class BaseTestFixture
    {
        protected BaseTestFixture(MobileTestPlatform mobileTestPlatform)
        {
            AppiumDriver.MobileTestPlatform = mobileTestPlatform;
        }
    }

    [TestFixture(MobileTestPlatform.Android, Category = "Android")]
    [TestFixture(MobileTestPlatform.iOS, Category = "iOS")]
    public partial class RedeemVoucherFeature : BaseTestFixture
    {
        public RedeemVoucherFeature(MobileTestPlatform platform)
            : base(platform)
        {
        }
    }

    [TestFixture(MobileTestPlatform.Android, Category = "Android")]
    [TestFixture(MobileTestPlatform.iOS, Category = "iOS")]
    public partial class LoginFeature : BaseTestFixture
    {
        public LoginFeature(MobileTestPlatform platform)
            : base(platform)
        {
        }
    }
}
