namespace VoucherRedemptionMobile.IntegrationTests.Pages
{
    using Common;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.IntegrationTests.Common.BasePage" />
    public class VoucherRedemptionFailedPage : BasePage
    {
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
                Android = x => x.Marked("REDEMPTION FAILURE"),
                iOS = x => x.Marked("REDEMPTION FAILURE")
            };

        #endregion
    }
}