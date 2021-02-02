using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoucherRedemptionMobile.IntegrationTests.Features
{
    using Common;
    using NUnit.Framework;
    using Xamarin.UITest;

    [TestFixture(Platform.Android, Category = "Android")]
    [TestFixture(Platform.iOS, Category = "iOS")]
    public partial class RedeemVoucherFeature : BaseTestFixture
    {
        public RedeemVoucherFeature(Platform platform)
            : base(platform)
        {
        }
    }
}
