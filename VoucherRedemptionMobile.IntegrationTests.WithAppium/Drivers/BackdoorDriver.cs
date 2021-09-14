namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using Features;
    using Newtonsoft.Json;
    using TestClients.Models;

    public class BackdoorDriver
    {
        public BackdoorDriver()
        {
        }

        public async Task SetIntegrationModeOn()
        {
            await this.ExecuteBackdoor("SetIntegrationTestModeOn", "");
        }

        public async Task AddTestVoucher(Voucher voucher)
        {
            String voucherData = JsonConvert.SerializeObject(voucher);
            await this.ExecuteBackdoor("AddTestVoucher", voucherData);
        }

        private async Task ExecuteBackdoor(string methodName, string value)
        {
            if (AppiumDriver.MobileTestPlatform == MobileTestPlatform.Android)
            {
                Dictionary<String, Object> args = BackdoorDriver.CreateBackdoorArgs(methodName, value);
                AppiumDriver.AndroidDriver.ExecuteScript("mobile: backdoor", args);
            }
            else
            {
                // Not required
            }
        }

        private static Dictionary<string, object> CreateBackdoorArgs(string methodName, string value)
        {
            return new Dictionary<string, object>
                   {
                       {"target", "application"},
                       {
                           "methods", new List<Dictionary<string, object>>
                                      {
                                          new Dictionary<string, object>
                                          {
                                              {"name", methodName},
                                              {
                                                  "args", new List<Dictionary<string, object>>
                                                          {
                                                              new Dictionary<string, object>
                                                              {
                                                                  {"value", value},
                                                                  {"type", "String"}
                                                              }
                                                          }
                                              }
                                          }
                                      }
                       }
                   };
        }
    }
}