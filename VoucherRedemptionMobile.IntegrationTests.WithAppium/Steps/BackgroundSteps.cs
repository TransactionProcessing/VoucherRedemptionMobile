using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Steps
{
    using System.IO;
    using System.IO.Compression;
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

        [Given(@"the following users exist")]
        public async Task GivenTheFollowingUsersExist(Table table)
        {
            foreach (TableRow tableRow in table.Rows)
            {
                String emailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress");
                String password = SpecflowTableHelper.GetStringRowValue(tableRow, "Password");

                await this.Backdoor.AddUserDetails((emailAddress, password));
                this.TestingContext.Users.Add((emailAddress, password));
            }
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
                
                await this.Backdoor.AddTestVoucher(voucher);
                this.TestingContext.Vouchers.Add(voucher);
            }
        }

        [Given(@"the application in in test mode")]
        public async Task GivenTheApplicationInInTestMode()
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.iOS)
            {
                String stage = null;
                try
                {
                    await this.LoginPage.AssertOnPage();
                    stage = "1";
                    await this.LoginPage.ClickTestModeButton();
                    stage = "2";
                    await this.TestModePage.AssertOnPage();
                    stage = "3";
                    await this.TestModePage.EnterPin("1234");
                    stage = "4";
                    var vouchers = this.TestingContext.Vouchers;
                    var voucherData = JsonConvert.SerializeObject(vouchers);
                    voucherData = StringCompression.Compress(voucherData);
                    await this.TestModePage.EnterTestVoucherData(voucherData);
                    stage = "5";
                    var usersList = this.TestingContext.Users;
                    var userData = JsonConvert.SerializeObject(usersList);
                    userData = StringCompression.Compress(userData);
                    await this.TestModePage.EnterTestUserData(userData);
                    stage = "6";
                    await this.TestModePage.ClickSetTestModeButton();
                }
                catch(Exception e)
                {
                    throw new Exception($"Failed to find element. Stage {stage}. Source [{AppiumDriver.iOSDriver.PageSource}]");
                }
            }
        }
    }

    public static class StringCompression
    {
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string Compress(string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    // setting the leaveOpen parameter to true to ensure that compressedStream will not be closed when compressorStream is disposed
                    // this allows compressorStream to close and flush its buffers to compressedStream and guarantees that compressedStream.ToArray() can be called afterward
                    // although MSDN documentation states that ToArray() can be called on a closed MemoryStream, I don't want to rely on that very odd behavior should it ever change
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }

                    // call compressedStream.ToArray() after the enclosing DeflateStream has closed and flushed its buffer to compressedStream
                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        public static string Decompress(string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }
    }
}
