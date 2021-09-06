using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Steps
{
    using System.Threading.Tasks;
    using Common;
    using Drivers;
    using Features;
    using Newtonsoft.Json;
    using Pages;
    using TechTalk.SpecFlow;
    using TestClients.Models;

    [Binding]
    [Scope(Tag = "background")]
    public class BackgroundSteps
    {
        private readonly BackdoorDriver Backdoor;

        private readonly ScenarioContext ScenarioContext;

        private readonly TestingContext TestingContext;

        private LoginPage LoginPage = new LoginPage();
        private TestModePage TestModePage = new TestModePage();

        public BackgroundSteps(BackdoorDriver backdoor,
                               ScenarioContext scenarioContext,
                               TestingContext testingContext)
        {
            this.Backdoor = backdoor;
            this.ScenarioContext = scenarioContext;
            this.TestingContext = testingContext;

            this.Backdoor.SetIntegrationModeOn().Wait();
        }

        [Given(@"the following vouchers have been issued")]
        public async Task GivenTheFollowingVouchersHaveBeenIssued(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                //| VoucherCode | VoucherValue | RecipientEmail                 | RecipientMobile |
                String voucherCode = SpecflowTableHelper.GetStringRowValue(tableRow, "VoucherCode");
                Decimal voucherValue = SpecflowTableHelper.GetDecimalValue(tableRow, "VoucherValue");
                String recipientEmail = String.IsNullOrEmpty(SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail"))
                    ? null
                    : SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail");
                String recipientMobile = String.IsNullOrEmpty(SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile"))
                    ? null
                    : SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile");

                Voucher voucher = new Voucher
                                  {
                                      Balance = voucherValue,
                                      Barcode = String.Empty, // TODO: Generate a barcode
                                      EstateId = Guid.Parse("347C8CD4-A194-4115-A36F-A75A5E24C49B"),
                                      ContractId = Guid.Parse("FC3E5F36-54AA-4BA2-8BF8-5391ACE4BD4B"),
                                      ExpiryDate = DateTime.Now.AddDays(30),
                                      GeneratedDateTime = DateTime.Now,
                                      IsGenerated = true,
                                      IsIssued = true,
                                      IsRedeemed = false,
                                      IssuedDateTime = DateTime.Now.AddSeconds(5),
                                      Message = String.Empty,
                                      RecipientEmail = recipientEmail,
                                      RecipientMobile = recipientMobile,
                                      RedeemedDateTime = DateTime.MinValue,
                                      TransactionId = Guid.NewGuid(),
                                      Value = voucherValue,
                                      VoucherCode = voucherCode,
                                      VoucherId = Guid.NewGuid()
                                  };
                
                //await this.Backdoor.AddTestVoucher(voucher);
                this.TestingContext.Vouchers.Add(voucher);
            }
        }

        [Given(@"the application in in test mode")]
        public async Task GivenTheApplicationInInTestMode()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                await this.LoginPage.ClickTestModeButton();

                var vouchers = this.TestingContext.Vouchers;
                var voucherData = JsonConvert.SerializeObject(vouchers);
                await this.TestModePage.EnterPin("1234");

                await this.TestModePage.EnterTestVoucherData(voucherData);
                await this.TestModePage.ClickSetTestModeButton();
            }
        }
    }
}
