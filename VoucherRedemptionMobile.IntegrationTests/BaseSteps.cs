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
        using VoucherManagement.DataTransferObjects;

        [Binding]
        [Scope(Tag = "base")]
        public class BaseSteps
        {

            private readonly FeatureContext FeatureContext;

            private readonly ScenarioContext ScenarioContext;

            private readonly TestingContext TestingContext;

            //    private IApp App;

            public BaseSteps(FeatureContext featureContext,
                             ScenarioContext scenarioContext,
                             TestingContext testingContext)
            {
                this.FeatureContext = featureContext;
                this.ScenarioContext = scenarioContext;
                this.TestingContext = testingContext;
            }

            [BeforeScenario()]
            public async Task StartSystem()
            {
                // Initialise a logger
                String scenarioName = this.ScenarioContext.ScenarioInfo.Title.Replace(" ", "");
                TestingLogger logger = new TestingLogger();

                this.TestingContext.DockerHelper = new VoucherRedemptionMobileDockerHelper(logger, this.TestingContext);
                logger.LogInformation($"About to Start Containers for Scenario Run - {scenarioName}");
                await this.TestingContext.DockerHelper.StartContainersForScenarioRun(scenarioName).ConfigureAwait(false);
                logger.LogInformation($"Containers for Scenario Run Started  - {scenarioName}");
            }

            [AfterScenario()]
            public async Task StopSystem()
            {
                TestingLogger logger = new TestingLogger();
                if (this.ScenarioContext.TestError != null)
                {
                    List<IContainerService> containers = this.TestingContext.DockerHelper.Containers
                                                             .Where(c => c.Name == this.TestingContext.DockerHelper.EstateManagementContainerName).ToList();

                    // The test has failed, grab the logs from all the containers
                    foreach (IContainerService containerService in containers)
                    {
                        ConsoleStream<String> logStream = containerService.Logs();
                        IList<String> logData = logStream.ReadToEnd();

                        foreach (String s in logData)
                        {
                            logger.LogInformation(s);
                        }
                    }
                }

                String scenarioName = this.ScenarioContext.ScenarioInfo.Title.Replace(" ", "");

                logger.LogInformation($"About to Stop Containers for Scenario Run - {scenarioName}");
                await this.TestingContext.DockerHelper.StopContainersForScenarioRun().ConfigureAwait(false);
                logger.LogInformation($"Containers for Scenario Run Stopped  - {scenarioName}");

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

        [Binding]
        [Scope(Tag = "shared")]
        public class SharedSteps
        {
            private readonly ScenarioContext ScenarioContext;

            private readonly TestingContext TestingContext;

            public SharedSteps(ScenarioContext scenarioContext,
                               TestingContext testingContext)
            {
                this.ScenarioContext = scenarioContext;
                this.TestingContext = testingContext;
            }

            [Given(@"the following security roles exist")]
            public async Task GivenTheFollowingSecurityRolesExist(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String roleName = SpecflowTableHelper.GetStringRowValue(tableRow, "RoleName");

                    CreateRoleRequest createRoleRequest = new CreateRoleRequest
                                                          {
                                                              RoleName = roleName
                                                          };

                    CreateRoleResponse createRoleResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                      .CreateRole(createRoleRequest, CancellationToken.None).ConfigureAwait(false);

                    createRoleResponse.RoleId.ShouldNotBe(Guid.Empty);
                }
            }

            [Given(@"the following api resources exist")]
            public async Task GivenTheFollowingApiResourcesExist(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String resourceName = SpecflowTableHelper.GetStringRowValue(tableRow, "ResourceName");
                    String displayName = SpecflowTableHelper.GetStringRowValue(tableRow, "DisplayName");
                    String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                    String scopes = SpecflowTableHelper.GetStringRowValue(tableRow, "Scopes");
                    String userClaims = SpecflowTableHelper.GetStringRowValue(tableRow, "UserClaims");

                    List<String> splitScopes = scopes.Split(',').ToList();
                    List<String> splitUserClaims = userClaims.Split(',').ToList();

                    CreateApiResourceRequest createApiResourceRequest = new CreateApiResourceRequest
                                                                        {
                                                                            Description = String.Empty,
                                                                            DisplayName = displayName,
                                                                            Name = resourceName,
                                                                            Scopes = new List<String>(),
                                                                            Secret = secret,
                                                                            UserClaims = new List<String>()
                                                                        };
                    splitScopes.ForEach(a => { createApiResourceRequest.Scopes.Add(a.Trim()); });
                    splitUserClaims.ForEach(a => { createApiResourceRequest.UserClaims.Add(a.Trim()); });

                    CreateApiResourceResponse createApiResourceResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                                    .CreateApiResource(createApiResourceRequest, CancellationToken.None)
                                                                                    .ConfigureAwait(false);

                    createApiResourceResponse.ApiResourceName.ShouldBe(resourceName);
                }
            }

            [Given(@"the following clients exist")]
            public async Task GivenTheFollowingClientsExist(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");
                    String clientName = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientName");
                    String secret = SpecflowTableHelper.GetStringRowValue(tableRow, "Secret");
                    String allowedScopes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedScopes");
                    String allowedGrantTypes = SpecflowTableHelper.GetStringRowValue(tableRow, "AllowedGrantTypes");

                    List<String> splitAllowedScopes = allowedScopes.Split(',').ToList();
                    List<String> splitAllowedGrantTypes = allowedGrantTypes.Split(',').ToList();

                    CreateClientRequest createClientRequest = new CreateClientRequest
                                                              {
                                                                  Secret = secret,
                                                                  AllowedGrantTypes = new List<String>(),
                                                                  AllowedScopes = new List<String>(),
                                                                  ClientDescription = String.Empty,
                                                                  ClientId = clientId,
                                                                  ClientName = clientName
                                                              };

                    splitAllowedScopes.ForEach(a => { createClientRequest.AllowedScopes.Add(a.Trim()); });
                    splitAllowedGrantTypes.ForEach(a => { createClientRequest.AllowedGrantTypes.Add(a.Trim()); });

                    CreateClientResponse createClientResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                          .CreateClient(createClientRequest, CancellationToken.None).ConfigureAwait(false);

                    createClientResponse.ClientId.ShouldBe(clientId);

                    this.TestingContext.AddClientDetails(clientId, secret, allowedGrantTypes);
                }

                //var merchantClient = this.TestingContext.GetClientDetails("merchantClient");

                //String securityService = this.TestingContext.DockerHelper.SecurityServiceBaseAddress;
                //String transactionProcessorAcl = this.TestingContext.DockerHelper.TransactionProcessorACLBaseAddress;
                //String estateManagementUrl = this.TestingContext.DockerHelper.EstateManagementBaseAddress;
                String mobileConfigUrl = this.TestingContext.DockerHelper.MobileConfigBaseAddress;

                //Console.WriteLine($"securityService [{securityService}]");
                //Console.WriteLine($"transactionProcessorAcl [{transactionProcessorAcl}]");
                //Console.WriteLine($"estateManagementUrl [{estateManagementUrl}]");
                //Console.WriteLine($"mobileConfigUrl [{mobileConfigUrl}]");

                // Setup the config host
                //var deviceIdentifier = AppManager.GetDeviceIdentifier();
                //DevelopmentConfiguration config = new DevelopmentConfiguration();
                //config.EnableAutoUpdates = false;
                //config.ClientId = merchantClient.ClientId;
                //config.ClientSecret = merchantClient.ClientSecret;
                //config.EstateManagement = estateManagementUrl;
                //config.SecurityService = securityService;
                //config.TransactionProcessorACL = transactionProcessorAcl;
                //config.DeviceIdentifier = AppManager.GetDeviceIdentifier();
                //config.id = AppManager.GetDeviceIdentifier();

                //Console.WriteLine(JsonConvert.SerializeObject(config));
                //Console.WriteLine($"Uri [{ mobileConfigUrl}/configuration]");

                //StringContent content = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");
                //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, $"{mobileConfigUrl}/configuration");
                //message.Content = content;

                //await this.TestingContext.DockerHelper.MobileConfigHttpClient.SendAsync(message, CancellationToken.None).ConfigureAwait(false);

                AppManager.SetConfiguration(mobileConfigUrl);
            }

            [Given(@"I have a token to access the estate management and voucher management resources")]
            public async Task GivenIHaveATokenToAccessTheEstateManagementAndVoucherManagementAclResources(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String clientId = SpecflowTableHelper.GetStringRowValue(tableRow, "ClientId");

                    Common.ClientDetails clientDetails = this.TestingContext.GetClientDetails(clientId);

                    if (clientDetails.GrantType == "client_credentials")
                    {
                        TokenResponse tokenResponse = await this.TestingContext.DockerHelper.SecurityServiceClient
                                                                .GetToken(clientId, clientDetails.ClientSecret, CancellationToken.None).ConfigureAwait(false);

                        this.TestingContext.AccessToken = tokenResponse.AccessToken;
                    }
                }
            }

            [Given(@"I have created the following estates")]
            [When(@"I create the following estates")]
            public async Task WhenICreateTheFollowingEstates(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String estateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName");

                    CreateEstateRequest createEstateRequest = new CreateEstateRequest
                                                              {
                                                                  EstateId = Guid.NewGuid(),
                                                                  EstateName = estateName
                                                              };

                    CreateEstateResponse response = null;
                    try
                    {

                        response = await this.TestingContext.DockerHelper.EstateClient
                                             .CreateEstate(this.TestingContext.AccessToken, createEstateRequest, CancellationToken.None).ConfigureAwait(false);
                    }
                    catch(Exception e)
                    {
                        this.TestingContext.DockerHelper.Logger.LogInformation(e.Message);
                        if (e.InnerException != null)
                        {
                            this.TestingContext.DockerHelper.Logger.LogInformation(e.InnerException.Message);
                            if (e.InnerException.InnerException != null)
                            {
                                this.TestingContext.DockerHelper.Logger.LogInformation(e.InnerException.InnerException.Message);
                            }
                        }
                    }

                    response.ShouldNotBeNull();
                    response.EstateId.ShouldNotBe(Guid.Empty);

                    // Cache the estate id
                    this.TestingContext.AddEstateDetails(response.EstateId, estateName);

                    //this.TestingContext.Logger.LogInformation($"Estate {estateName} created with Id {response.EstateId}");
                }

                foreach (TableRow tableRow in table.Rows)
                {
                    EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);
                    EstateResponse estate = null;
                    await Retry.For(async () =>
                                    {
                                        estate = await this.TestingContext.DockerHelper.EstateClient
                                                           .GetEstate(this.TestingContext.AccessToken, estateDetails.EstateId, CancellationToken.None)
                                                           .ConfigureAwait(false);
                                    },
                                    TimeSpan.FromMinutes(2));

                    estate.ShouldNotBeNull();
                    estate.EstateName.ShouldBe(estateDetails.EstateName);
                }
            }

            [Given(@"I have created the following operators")]
            [When(@"I create the following operators")]
            public async Task WhenICreateTheFollowingOperators(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    String operatorName = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName");
                    Boolean requireCustomMerchantNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomMerchantNumber");
                    Boolean requireCustomTerminalNumber = SpecflowTableHelper.GetBooleanValue(tableRow, "RequireCustomTerminalNumber");

                    CreateOperatorRequest createOperatorRequest = new CreateOperatorRequest
                                                                  {
                                                                      Name = operatorName,
                                                                      RequireCustomMerchantNumber = requireCustomMerchantNumber,
                                                                      RequireCustomTerminalNumber = requireCustomTerminalNumber
                                                                  };

                    // lookup the estate id based on the name in the table
                    EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                    CreateOperatorResponse response = await this.TestingContext.DockerHelper.EstateClient
                                                                .CreateOperator(this.TestingContext.AccessToken,
                                                                                estateDetails.EstateId,
                                                                                createOperatorRequest,
                                                                                CancellationToken.None).ConfigureAwait(false);

                    response.ShouldNotBeNull();
                    response.EstateId.ShouldNotBe(Guid.Empty);
                    response.OperatorId.ShouldNotBe(Guid.Empty);

                    // Cache the estate id
                    estateDetails.AddOperator(response.OperatorId, operatorName);

                    //this.TestingContext.Logger.LogInformation($"Operator {operatorName} created with Id {response.OperatorId} for Estate {estateDetails.EstateName}");
                }
            }

            [When(@"I create the following security users")]
            [Given("I have created the following security users")]
            public async Task WhenICreateTheFollowingSecurityUsers(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    // lookup the estate id based on the name in the table
                    EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                    String roleName = SpecflowTableHelper.GetStringRowValue(tableRow, "RoleName");
                    String emailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "EmailAddress");
                    String password = SpecflowTableHelper.GetStringRowValue(tableRow, "Password");
                    String givenName = SpecflowTableHelper.GetStringRowValue(tableRow, "GivenName");
                    String familyName = SpecflowTableHelper.GetStringRowValue(tableRow, "FamilyName");
                    
                    CreateUserRequest createUserRequest = new CreateUserRequest
                    {
                        EmailAddress = emailAddress,
                        FamilyName = familyName,
                        GivenName = givenName,
                        MiddleName = String.Empty,
                        Password = password,
                        PhoneNumber = "123456789",
                        Roles = new List<String>
                                                                  {
                                                                      roleName
                                                                  },
                        Claims = new Dictionary<String, String>
                                                                   {
                                                                       {"EstateId", estateDetails.EstateId.ToString()},
                                                                       {"ContractId", estateDetails.EstateId.ToString()}
                                                                   }
                    };

                    CreateUserResponse createUserResponse = await this.TestingContext.DockerHelper.SecurityServiceClient.CreateUser(createUserRequest, CancellationToken.None)
                                                                      .ConfigureAwait(false);

                    createUserResponse.UserId.ShouldNotBe(Guid.Empty);
                }
            }

            [When(@"I issue the following vouchers")]
            public async Task WhenIIssueTheFollowingVouchers(Table table)
            {
                foreach (TableRow tableRow in table.Rows)
                {
                    EstateDetails estateDetails = this.TestingContext.GetEstateDetails(tableRow);

                    IssueVoucherRequest request = new IssueVoucherRequest
                                                  {
                                                      Value = SpecflowTableHelper.GetDecimalValue(tableRow, "Value"),
                                                      RecipientEmail = SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientEmail"),
                                                      RecipientMobile = SpecflowTableHelper.GetStringRowValue(tableRow, "RecipientMobile"),
                                                      EstateId = estateDetails.EstateId,
                                                      OperatorIdentifier = SpecflowTableHelper.GetStringRowValue(tableRow, "OperatorName"),
                                                      TransactionId = Guid.Parse(SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionId"))
                                                  };

                    IssueVoucherResponse response = await this.TestingContext.DockerHelper.VoucherManagementClient.IssueVoucher(this.TestingContext.AccessToken, request, CancellationToken.None)
                                                              .ConfigureAwait(false);

                    response.VoucherId.ShouldNotBe(Guid.Empty);

                    estateDetails.AddVoucher(request.OperatorIdentifier,
                                             request.Value,
                                             request.TransactionId,
                                             response.VoucherCode,
                                             response.VoucherId,
                                             response.ExpiryDate);
                }
            }
        }
    }

}
