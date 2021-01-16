using TechTalk.SpecFlow;

namespace VoucherRedemptionMobile.IntegrationTests.Features
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Pages;

    /// <summary>
    /// 
    /// </summary>
    [Binding]
    [Scope(Tag = "redeemvoucher")]
    public class RedeemVoucherSteps
    {
        /// <summary>
        /// The testing context
        /// </summary>
        private readonly TestingContext TestingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedeemVoucherSteps"/> class.
        /// </summary>
        /// <param name="testingContext">The testing context.</param>
        public RedeemVoucherSteps(TestingContext testingContext)
        {
            this.TestingContext = testingContext;
        }

        /// <summary>
        /// The main page
        /// </summary>
        MainPage mainPage = new MainPage();

        /// <summary>
        /// The vouchers page
        /// </summary>
        VouchersPage vouchersPage = new VouchersPage();

        /// <summary>
        /// The voucher redemption page
        /// </summary>
        VoucherRedemptionPage voucherRedemptionPage = new VoucherRedemptionPage();

        /// <summary>
        /// The voucher details page
        /// </summary>
        VoucherDetailsPage voucherDetailsPage = new VoucherDetailsPage();
        /// <summary>
        /// The voucher redemption success page
        /// </summary>
        VoucherRedemptionSuccessPage voucherRedemptionSuccessPage = new VoucherRedemptionSuccessPage();
        /// <summary>
        /// The voucher redemption failed page
        /// </summary>
        VoucherRedemptionFailedPage voucherRedemptionFailedPage = new VoucherRedemptionFailedPage();

        /// <summary>
        /// Thens the home page is displayed.
        /// </summary>
        [Then(@"the Home Page is displayed")]
        public async Task ThenTheHomePageIsDisplayed()
        {
            await this.mainPage.AssertOnPage();
        }

        /// <summary>
        /// Givens the i tap on the vouchers button.
        /// </summary>
        [Given(@"I tap on the Vouchers button")]
        public void GivenITapOnTheVouchersButton()
        {
            this.mainPage.ClickVouchersButton();
        }

        /// <summary>
        /// Thens the vouchers page is displayed.
        /// </summary>
        [Then(@"the Vouchers Page is displayed")]
        public async Task ThenTheVouchersPageIsDisplayed()
        {
            await this.vouchersPage.AssertOnPage();
        }

        /// <summary>
        /// Givens the i tap on the redeem voucher button.
        /// </summary>
        [Given(@"I tap on the Redeem Voucher button")]
        public void GivenITapOnTheRedeemVoucherButton()
        {
            this.vouchersPage.ClickRedeemVoucherButton();
        }

        /// <summary>
        /// Thens the voucher redemption page is displayed.
        /// </summary>
        [Then(@"the Voucher Redemption Page is displayed")]
        public async Task ThenTheVoucherRedemptionPageIsDisplayed()
        {
            await this.voucherRedemptionPage.AssertOnPage();
        }

        /// <summary>
        /// Givens the i enter the voucher code for the voucher for.
        /// </summary>
        /// <param name="voucherValue">The voucher value.</param>
        /// <param name="estateName">Name of the estate.</param>
        [Given(@"I enter the voucher code for the (.*) voucher for '(.*)'")]
        public void GivenIEnterTheVoucherCodeForTheVoucherFor(Decimal voucherValue,
                                                              String estateName)
        {
            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            // find the voucher
            (Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime) voucher = estateDetails.GetVoucher(voucherValue);

            this.voucherRedemptionPage.EnterVoucherCode(voucher.voucherCode);
        }

        /// <summary>
        /// Givens the i tap on the find voucher button.
        /// </summary>
        [Given(@"I tap on the Find Voucher Button")]
        public void GivenITapOnTheFindVoucherButton()
        {
            this.voucherRedemptionPage.ClickFindVoucherButton();
        }

        /// <summary>
        /// Thens the voucher details are displayed for the voucher for.
        /// </summary>
        /// <param name="voucherValue">The voucher value.</param>
        /// <param name="estateName">Name of the estate.</param>
        [Then(@"the voucher details are displayed for the (.*) voucher for '(.*)'")]
        public async Task ThenTheVoucherDetailsAreDisplayedForTheVoucherFor(Decimal voucherValue,
                                                                            String estateName)
        {
            await this.voucherDetailsPage.AssertOnPage();

            EstateDetails estateDetails = this.TestingContext.GetEstateDetails(estateName);

            // find the voucher
            (Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime) voucher = estateDetails.GetVoucher(voucherValue);

            this.voucherDetailsPage.AssertVoucherDetails(voucher.voucherCode, voucher.value, voucher.value, voucher.expiryDateTime);
        }

        /// <summary>
        /// Whens the i tap on the redeem button.
        /// </summary>
        [When(@"I tap on the Redeem Button")]
        public void WhenITapOnTheRedeemButton()
        {
            this.voucherDetailsPage.ClickRedeemVoucherButton();
        }

        /// <summary>
        /// Thens the voucher redemption successful screen will be displayed.
        /// </summary>
        [Then(@"The Voucher Redemption Successful Screen will be displayed")]
        public async Task ThenTheVoucherRedemptionSuccessfulScreenWillBeDisplayed()
        {
            await this.voucherRedemptionSuccessPage.AssertOnPage();
        }


    }
}
