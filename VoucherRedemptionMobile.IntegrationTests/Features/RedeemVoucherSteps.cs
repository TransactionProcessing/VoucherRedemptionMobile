using TechTalk.SpecFlow;

namespace VoucherRedemptionMobile.IntegrationTests.Features
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Pages;
    using Shouldly;
    using TestClients;
    using TestClients.Models;
    using TransactionMobile.IntegrationTests;
    using VoucherRedemption.Clients;

    /// <summary>
    /// 
    /// </summary>
    [Binding]
    [Scope(Tag = "redeemvoucher")]
    public class RedeemVoucherSteps
    {
        private readonly TestingContext TestingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedeemVoucherSteps"/> class.
        /// </summary>
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
        
        [Given(@"I enter the voucher code '(.*)' voucher")]
        public void GivenIEnterTheVoucherCodeVoucher(String voucherCode)
        {
            this.voucherRedemptionPage.EnterVoucherCode(voucherCode);
        }
        
        /// <summary>
        /// Givens the i tap on the find voucher button.
        /// </summary>
        [Given(@"I tap on the Find Voucher Button")]
        public void GivenITapOnTheFindVoucherButton()
        {
            this.voucherRedemptionPage.ClickFindVoucherButton();
        }
        
        [Then(@"the voucher details are displayed for the voucher with code '(.*)'")]
        public void ThenTheVoucherDetailsAreDisplayedForTheVoucherWithCode(String voucherCode)
        {
            Voucher voucher = this.TestingContext.Vouchers.SingleOrDefault(v => v.VoucherCode == voucherCode);

            voucher.ShouldNotBeNull();

            this.voucherDetailsPage.AssertVoucherDetails(voucher.VoucherCode, voucher.Value, voucher.Value, voucher.ExpiryDate);
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
