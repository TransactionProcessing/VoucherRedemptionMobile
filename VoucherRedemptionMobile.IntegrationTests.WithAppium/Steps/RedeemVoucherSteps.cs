using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Steps
{
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Pages;
    using Shouldly;
    using TechTalk.SpecFlow;
    using TestClients.Models;

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
        /// Givens the i tap on the vouchers button.
        /// </summary>
        [Given(@"I tap on the Vouchers button")]
        public async Task GivenITapOnTheVouchersButton()
        {
            await this.mainPage.ClickVouchersButton();
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
        public async Task GivenITapOnTheRedeemVoucherButton()
        {
            await this.vouchersPage.ClickRedeemVoucherButton();
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
        public async Task GivenIEnterTheVoucherCodeVoucher(String voucherCode)
        {
            await this.voucherRedemptionPage.EnterVoucherCode(voucherCode);
        }

        /// <summary>
        /// Givens the i tap on the find voucher button.
        /// </summary>
        [Given(@"I tap on the Find Voucher Button")]
        public async Task GivenITapOnTheFindVoucherButton()
        {
            await this.voucherRedemptionPage.ClickFindVoucherButton();
        }

        [Then(@"the voucher details are displayed for the voucher with code '(.*)'")]
        public async Task ThenTheVoucherDetailsAreDisplayedForTheVoucherWithCode(String voucherCode)
        {
            Voucher voucher = this.TestingContext.Vouchers.SingleOrDefault(v => v.VoucherCode == voucherCode);

            voucher.ShouldNotBeNull();

            await this.voucherDetailsPage.AssertVoucherDetails(voucher.VoucherCode, voucher.Value, voucher.Value, voucher.ExpiryDate);
        }

        /// <summary>
        /// Whens the i tap on the redeem button.
        /// </summary>
        [When(@"I tap on the Redeem Button")]
        public async Task WhenITapOnTheRedeemButton()
        {
            await this.voucherDetailsPage.ClickRedeemVoucherButton();
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
