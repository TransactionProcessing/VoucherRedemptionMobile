using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace TransactionMobile.IntegrationTests
    {
        using System.Diagnostics;
        using System.Diagnostics.Contracts;
        using System.IO;
        using System.Net;
        using System.Net.Http;
        using System.Net.Sockets;
        using System.Reflection;
        using System.Threading;
        using Common;
        using Ductus.FluentDocker.Executors;
        using Ductus.FluentDocker.Extensions;
        using Ductus.FluentDocker.Services;
        using Ductus.FluentDocker.Services.Extensions;
        using EstateManagement.DataTransferObjects;
        using EstateManagement.DataTransferObjects.Requests;
        using EstateManagement.DataTransferObjects.Responses;
        using Newtonsoft.Json;
        using SecurityService.DataTransferObjects;
        using SecurityService.DataTransferObjects.Requests;
        using SecurityService.DataTransferObjects.Responses;
        using Shouldly;
        using TechTalk.SpecFlow;
        using TestClients;
        using TestClients.Models;

        [Binding]
        [Scope(Tag = "base")]
        public class BaseSteps
        {

            private readonly FeatureContext FeatureContext;

            private readonly ScenarioContext ScenarioContext;

            private readonly TestingContext TestingContext;

            public BaseSteps(FeatureContext featureContext,
                             ScenarioContext scenarioContext,
                             TestingContext testingContext)
            {
                this.FeatureContext = featureContext;
                this.ScenarioContext = scenarioContext;
                this.TestingContext = testingContext;
            }

            //[AfterStep]
            public void CheckStepStatus()
            {
                // Build the screenshot name
                String featureName = this.FeatureContext.GetFeatureNameForScreenshot();
                String scenarioName = this.ScenarioContext.GetScenarioNameForScreenshot();
                String stepName = this.ScenarioContext.GetStepNameForScreenshot();

                // Capture screen shot on exception
                //FileInfo screenshot = AppManager.App.Screenshot($"{scenarioName}:{stepName}");
                FileInfo screenshot = null;

                String screenshotPath = Environment.GetEnvironmentVariable("ScreenshotFolder");
                if (String.IsNullOrEmpty(screenshotPath))
                {
                    // Get the executing directory
                    String currentDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";

                    String screenshotDirectory = $"{currentDirectory}\\Screenshots\\{featureName}";

                    if (!Directory.Exists(screenshotDirectory))
                    {
                        Directory.CreateDirectory(screenshotDirectory);
                    }

                    // Now copy the screenshot
                    FileInfo fi = screenshot.CopyTo($"{screenshotDirectory}\\{DateTime.Now:yyyMMddHHmmssfff}-{scenarioName}-{stepName}.jpg", true);

                    Console.WriteLine($"{fi.FullName} exists");
                }
                else
                {
                    screenshotPath = $"{screenshotPath}//{featureName}";
                    if (!Directory.Exists(screenshotPath))
                    {
                        Directory.CreateDirectory(screenshotPath);
                    }

                    String fileName = $"{screenshotPath}//{DateTime.Now:yyyMMddHHmmssfff}-{scenarioName}-{stepName}.jpg";
                    Console.WriteLine($"About to copy to {fileName}");
                    FileInfo fi = screenshot.CopyTo(fileName, true);
                    Console.WriteLine($"{fi.FullName} exists");
                }
            }

            [Given(@"the following vouchers have been issued")]
            public void GivenTheFollowingVouchersHaveBeenIssued(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    //| VoucherCode | VoucherValue | RecipientEmail                 | RecipientMobile |
                    String voucherCode = SpecflowTableHelper.GetStringRowValue(tableRow, "VoucherCode");
                    Decimal voucherValue = SpecflowTableHelper.GetDecimalValue(tableRow, "VoucherValue");
                    String recipientEmail = String.IsNullOrEmpty(SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail")) ? null : SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail");
                    String recipientMobile = String.IsNullOrEmpty(SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile")) ? null : SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile");

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

                    String voucherData = JsonConvert.SerializeObject(voucher);

                    AppManager.AddTestVoucher(voucherData);
                    this.TestingContext.Vouchers.Add(voucher);
                }
            }
        }

        public static class Extensions
        {
            public static String GetFeatureNameForLogging(this FeatureContext featureContext)
            {
                return featureContext.FeatureInfo.Title.Replace(" ", "");
            }

            public static String GetFeatureNameForScreenshot(this FeatureContext featureContext)
            {
                String featureName = featureContext.GetFeatureNameForLogging();
                featureName = String.Join("", featureName.Split(Path.GetInvalidFileNameChars()));

                // Remove other characters that are valid for a path but not a screenshot name (/ and : for example)
                featureName = featureName.Replace("/", "");
                featureName = featureName.Replace(":", "");

                return featureName;
            }

            public static String GetScenarioNameForLogging(this ScenarioContext scenarioContext)
            {
                return scenarioContext.ScenarioInfo.Title.Replace(" ", "");
            }

            /// <summary>
            /// Gets the scenario name for screenshot.
            /// </summary>
            /// <param name="scenarioContext">The scenario context.</param>
            /// <returns></returns>
            public static String GetScenarioNameForScreenshot(this ScenarioContext scenarioContext)
            {
                String scenarioName = scenarioContext.GetScenarioNameForLogging();

                scenarioName = String.Join("", scenarioName.Split(Path.GetInvalidFileNameChars()));

                // Remove other characters that are valid for a path but not a screenshot name (/ and : for example)
                scenarioName = scenarioName.Replace("/", "");
                scenarioName = scenarioName.Replace(":", "");

                return scenarioName;
            }

            /// <summary>
            /// Gets the step name for logging.
            /// </summary>
            /// <param name="scenarioContext">The scenario context.</param>
            /// <returns></returns>
            public static String GetStepNameForLogging(this ScenarioContext scenarioContext)
            {
                return scenarioContext.StepContext.StepInfo.Text.Replace(" ", "");
            }

            /// <summary>
            /// Gets the step name for screenshot.
            /// </summary>
            /// <param name="scenarioContext">The scenario context.</param>
            /// <returns></returns>
            public static String GetStepNameForScreenshot(this ScenarioContext scenarioContext)
            {
                String stepName = scenarioContext.GetStepNameForLogging();

                stepName = String.Join("", stepName.Split(Path.GetInvalidFileNameChars()));

                // Remove other characters that are valid for a path but not a screenshot name (/ and : for example)
                stepName = stepName.Replace("/", "");
                stepName = stepName.Replace(":", "");

                return stepName;
            }
        }

        public class TestingContext
        {
            public List<Voucher> Vouchers = new List<Voucher>();
        }
    }

}
