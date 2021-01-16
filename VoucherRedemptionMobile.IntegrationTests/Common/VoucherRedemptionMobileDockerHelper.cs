namespace VoucherRedemptionMobile.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Common;
    using Ductus.FluentDocker.Model.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using EstateManagement.Client;
    using Newtonsoft.Json;
    using SecurityService.Client;
    using Shouldly;
    using VoucherManagement.Client;

    public class VoucherRedemptionMobileDockerHelper
    {
        #region Fields

        public HttpClient ACLHttpClient;

        public List<IContainerService> Containers;

        public static IContainerService DatabaseServerContainer;

        public static INetworkService DatabaseServerNetwork;

        public IEstateClient EstateClient;

        public String EstateManagementContainerName;

        public readonly TestingLogger Logger;

        public String MobileConfigBaseAddress;

        public HttpClient MobileConfigHttpClient;

        public String SecurityServiceBaseAddress;

        public ISecurityServiceClient SecurityServiceClient;

        public static String SqlServerContainerName = "shareddatabasesqlserver";

        public Guid TestId;

        public String VoucherManagementACLBaseAddress;

        public String VoucherManagementBaseAddress;

        public IVoucherManagementClient VoucherManagementClient;

        protected Int32 EstateManagementApiPort;

        protected String EstateReportingContainerName;

        protected String EventStoreContainerName;

        protected Int32 EventStoreHttpPort;

        protected String MobileConfigurationContainerName;

        protected Int32 MobileConfigurationContainerPort;

        protected String SecurityServiceContainerName;

        protected Int32 SecurityServicePort;

        protected String SubscriptionServiceContainerName;

        protected readonly TestingContext TestingContext;

        protected List<INetworkService> TestNetworks;

        protected String VoucherManagementACLContainerName;

        protected Int32 VoucherManagementACLPort;

        protected String VoucherManagementContainerName;

        protected Int32 VoucherManagementPort;

        private static String LocalHostAddress;

        #endregion

        #region Constructors

        public VoucherRedemptionMobileDockerHelper(TestingLogger logger,
                                                   TestingContext testingContext)
        {
            this.Logger = logger;
            this.TestingContext = testingContext;
            this.Containers = new List<IContainerService>();
            this.TestNetworks = new List<INetworkService>();
        }

        #endregion

        #region Methods

        public static String GetConnectionString(String databaseName)
        {
            return
                $"server={VoucherRedemptionMobileDockerHelper.DatabaseServerContainer.Name};database={databaseName};user id={VoucherRedemptionMobileDockerHelper.SqlUserName};password={VoucherRedemptionMobileDockerHelper.SqlPassword}";
        }

        public static String GetLocalConnectionString(String databaseName)
        {
            Int32 databaseHostPort = VoucherRedemptionMobileDockerHelper.DatabaseServerContainer.ToHostExposedEndpoint("1433/tcp").Port;

            return
                $"server=localhost,{databaseHostPort};database={databaseName};user id={VoucherRedemptionMobileDockerHelper.SqlUserName};password={VoucherRedemptionMobileDockerHelper.SqlPassword}";
        }

        public static IContainerService SetupEstateManagementContainer(String containerName,
                                                                       ILogger logger,
                                                                       String imageName,
                                                                       List<INetworkService> networkServices,
                                                                       String hostFolder,
                                                                       (String URL, String UserName, String Password)? dockerCredentials,
                                                                       String securityServiceContainerName,
                                                                       String eventStoreContainerName,
                                                                       String sqlServerContainerName,
                                                                       String sqlServerUserName,
                                                                       String sqlServerPassword,
                                                                       (String clientId, String clientSecret) clientDetails,
                                                                       Boolean forceLatestImage = false,
                                                                       Int32 securityServicePort = VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort)
        {
            logger.LogInformation("About to Start Estate Management Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables
                .Add($"EventStoreSettings:ConnectionString=https://{eventStoreContainerName}:{VoucherRedemptionMobileDockerHelper.EventStoreHttpDockerPort}");
            environmentVariables.Add($"AppSettings:SecurityService=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"SecurityConfiguration:Authority=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"urls=http://*:{VoucherRedemptionMobileDockerHelper.EstateManagementDockerPort}");
            environmentVariables
                .Add($"ConnectionStrings:EstateReportingReadModel=\"server={sqlServerContainerName};user id={sqlServerUserName};password={sqlServerPassword};database=EstateReportingReadModel\"");

            ContainerBuilder estateManagementContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                      .UseImage(imageName, forceLatestImage)
                                                                      .ExposePort(VoucherRedemptionMobileDockerHelper.EstateManagementDockerPort)
                                                                      .UseNetwork(networkServices.ToArray()).Mount(hostFolder, "/home", MountType.ReadWrite);

            if (dockerCredentials.HasValue)
            {
                estateManagementContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = estateManagementContainer.Build().Start();

            logger.LogInformation("Estate Management Container Started");

            return builtContainer;
        }

        public static IContainerService SetupEstateReportingContainer(String containerName,
                                                                      ILogger logger,
                                                                      String imageName,
                                                                      List<INetworkService> networkServices,
                                                                      String hostFolder,
                                                                      (String URL, String UserName, String Password)? dockerCredentials,
                                                                      String securityServiceContainerName,
                                                                      String sqlServerContainerName,
                                                                      String sqlServerUserName,
                                                                      String sqlServerPassword,
                                                                      (String clientId, String clientSecret) clientDetails,
                                                                      Boolean forceLatestImage = false,
                                                                      Int32 securityServicePort = VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort)
        {
            logger.LogInformation("About to Start Estate Reporting Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables.Add($"AppSettings:SecurityService=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"SecurityConfiguration:Authority=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"urls=http://*:{VoucherRedemptionMobileDockerHelper.EstateReportingDockerPort}");
            environmentVariables
                .Add($"ConnectionStrings:EstateReportingReadModel=\"server={sqlServerContainerName};user id={sqlServerUserName};password={sqlServerPassword};database=EstateReportingReadModel\"");

            ContainerBuilder estateReportingContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                     .UseImage(imageName, forceLatestImage)
                                                                     .ExposePort(VoucherRedemptionMobileDockerHelper.EstateReportingDockerPort)
                                                                     .UseNetwork(networkServices.ToArray()).Mount(hostFolder, "/home", MountType.ReadWrite);

            if (dockerCredentials.HasValue)
            {
                estateReportingContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = estateReportingContainer.Build().Start();

            logger.LogInformation("Estate Reporting Container Started");

            return builtContainer;
        }

        /// <summary>
        /// Setups the event store container.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="networkService">The network service.</param>
        /// <param name="hostFolder">The host folder.</param>
        /// <param name="forceLatestImage">if set to <c>true</c> [force latest image].</param>
        /// <returns></returns>
        public static async Task<IContainerService> SetupEventStoreContainer(String containerName,
                                                                             ILogger logger,
                                                                             String imageName,
                                                                             INetworkService networkService,
                                                                             String hostFolder,
                                                                             Boolean forceLatestImage = false,
                                                                             Boolean usesEventStore2006OrLater = false)
        {
            logger.LogInformation("About to Start Event Store Container");

            List<String> enviromentVariables = new List<String>();
            enviromentVariables.Add("EVENTSTORE_RUN_PROJECTIONS=all");
            enviromentVariables.Add("EVENTSTORE_START_STANDARD_PROJECTIONS=true");

            if (usesEventStore2006OrLater)
            {
                enviromentVariables.Add("EVENTSTORE_DEV=true");
                enviromentVariables.Add("EVENTSTORE_ENABLE_EXTERNAL_TCP=true");
                enviromentVariables.Add("EVENTSTORE_DISABLE_EXTERNAL_TCP_TLS=true");
                enviromentVariables.Add("EVENTSTORE_ENABLE_ATOM_PUB_OVER_HTTP=true");
            }

            IContainerService eventStoreContainer = new Builder().UseContainer().UseImage(imageName, forceLatestImage)
                                                                 .ExposePort(VoucherRedemptionMobileDockerHelper.EventStoreHttpDockerPort)
                                                                 .WithEnvironment(enviromentVariables.ToArray())
                                                                 .ExposePort(VoucherRedemptionMobileDockerHelper.EventStoreTcpDockerPort).WithName(containerName)
                                                                 .UseNetwork(networkService).Mount(hostFolder, "/var/log/eventstore", MountType.ReadWrite).Build()
                                                                 .Start();

            await Task.Delay(20000);

            Int32 eventStoreHttpPort = eventStoreContainer.ToHostExposedEndpoint("2113/tcp").Port;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender,
                                                                       cert,
                                                                       chain,
                                                                       sslPolicyErrors) =>
                                                                      {
                                                                          return true;
                                                                      };

            // Verify the Event Store is running
            await Retry.For(async () =>
                            {
                                String url = $"https://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{eventStoreHttpPort}/ping";

                                HttpClient client = new HttpClient(clientHandler);

                                HttpResponseMessage pingResponse = await client.GetAsync(url).ConfigureAwait(false);
                                pingResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
                            }).ConfigureAwait(false);

            await Retry.For(async () =>
                            {
                                String url = $"https://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{eventStoreHttpPort}/info";

                                HttpClient client = new HttpClient(clientHandler);

                                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Authorization", "Basic YWRtaW46Y2hhbmdlaXQ=");

                                HttpResponseMessage infoResponse = await client.SendAsync(requestMessage, CancellationToken.None).ConfigureAwait(false);

                                infoResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
                                String infoData = await infoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                                logger.LogInformation(infoData);
                            }).ConfigureAwait(false);

            logger.LogInformation("Event Store Container Started");

            return eventStoreContainer;
        }

        public static async Task<IContainerService> SetupMobileConfigurationContainer(String containerName,
                                                                                      ILogger logger,
                                                                                      String imageName,
                                                                                      INetworkService networkService)
        {
            logger.LogInformation("About to Start Mobile Config Container");

            String executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            String config = $"{executableLocation}/Common/config/db.json";

            IContainerService mobileConfigContainer = new Builder().UseContainer().UseImage(imageName).ExposePort(80).WithName(containerName).UseNetwork(networkService)
                                                                   .CopyOnStart(config, "/data").Build().Start();

            Thread.Sleep(1000);
            logger.LogInformation("Mobile Config Container Started");

            return mobileConfigContainer;
        }

        public static IContainerService SetupSecurityServiceContainer(String containerName,
                                                                      ILogger logger,
                                                                      String imageName,
                                                                      INetworkService networkService,
                                                                      String hostFolder,
                                                                      (String URL, String UserName, String Password)? dockerCredentials,
                                                                      Boolean forceLatestImage = false)
        {
            logger.LogInformation("About to Start Security Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables.Add($"ServiceOptions:PublicOrigin=http://{containerName}:{VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort}");
            environmentVariables.Add($"ServiceOptions:IssuerUrl=http://{containerName}:{VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort}");
            environmentVariables.Add("ASPNETCORE_ENVIRONMENT=IntegrationTest");
            environmentVariables.Add("urls=http://*:5001");

            ContainerBuilder securityServiceContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                     .UseImage(imageName, forceLatestImage)
                                                                     .ExposePort(VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort)
                                                                     .UseNetwork(new List<INetworkService>
                                                                                 {
                                                                                     networkService
                                                                                 }.ToArray()).Mount(hostFolder, "/home/txnproc/trace", MountType.ReadWrite);

            if (dockerCredentials.HasValue)
            {
                securityServiceContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = securityServiceContainer.Build().Start();
            Thread.Sleep(20000); // This hack is in till health checks implemented :|

            logger.LogInformation("Security Service Container Started");

            return builtContainer;
        }

        public static IContainerService SetupSubscriptionServiceContainer(String containerName,
                                                                          ILogger logger,
                                                                          String imageName,
                                                                          List<INetworkService> networkServices,
                                                                          String hostFolder,
                                                                          (String URL, String UserName, String Password)? dockerCredentials,
                                                                          String securityServiceContainerName,
                                                                          String sqlServerContainerName,
                                                                          String sqlServerUserName,
                                                                          String sqlServerPassword,
                                                                          Guid eventStoreServerId,
                                                                          (String clientId, String clientSecret) clientDetails,
                                                                          Boolean forceLatestImage = false,
                                                                          Int32 securityServicePort = VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort)
        {
            logger.LogInformation("About to Start Subscription Service Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables.Add($"AppSettings:SecurityService=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"AppSettings:EventStoreServerId={eventStoreServerId}");
            environmentVariables.Add($"SecurityConfiguration:Authority=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables
                .Add($"ConnectionStrings:SubscriptionService=\"server={sqlServerContainerName};user id={sqlServerUserName};password={sqlServerPassword};database=SubscriptionServiceConfiguration\"");

            ContainerBuilder subscriptionServiceContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                         .UseImage(imageName, forceLatestImage).UseNetwork(networkServices.ToArray())
                                                                         .Mount(hostFolder, "/home", MountType.ReadWrite);

            if (dockerCredentials.HasValue)
            {
                subscriptionServiceContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = subscriptionServiceContainer.Build().Start();

            logger.LogInformation("Subscription Service Container Started");

            return builtContainer;
        }

        public static INetworkService SetupTestNetwork(String networkName = null,
                                                       Boolean reuseIfExists = false)
        {
            networkName = string.IsNullOrEmpty(networkName) ? $"testnetwork{Guid.NewGuid()}" : networkName;

            // Build a network
            NetworkBuilder networkService = new Builder().UseNetwork(networkName);

            if (reuseIfExists)
            {
                networkService.ReuseIfExist();
            }

            return networkService.Build();
        }

        public static IContainerService SetupVoucherManagementACLContainer(String containerName,
                                                                           ILogger logger,
                                                                           String imageName,
                                                                           List<INetworkService> networkServices,
                                                                           String hostFolder,
                                                                           (String URL, String UserName, String Password)? dockerCredentials,
                                                                           String securityServiceContainerName,
                                                                           String voucherManagementContainerName,
                                                                           (String clientId, String clientSecret) clientDetails,
                                                                           Boolean forceLatestImage = false,
                                                                           Int32 securityServicePort = VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort,
                                                                           List<String> additionalEnvironmentVariables = null)
        {
            logger.LogInformation("About to Start Voucher Management ACL Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables.Add($"AppSettings:SecurityService=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables
                .Add($"AppSettings:VoucherManagementApi=http://{voucherManagementContainerName}:{VoucherRedemptionMobileDockerHelper.VoucherManagementDockerPort}");
            environmentVariables.Add($"SecurityConfiguration:Authority=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"urls=http://*:{VoucherRedemptionMobileDockerHelper.VoucherManagementACLDockerPort}");
            environmentVariables.Add($"AppSettings:ClientId={clientDetails.clientId}");
            environmentVariables.Add($"AppSettings:ClientSecret={clientDetails.clientSecret}");

            if (additionalEnvironmentVariables != null)
            {
                environmentVariables.AddRange(additionalEnvironmentVariables);
            }

            ContainerBuilder voucherManagementAclContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                          .UseImage(imageName, forceLatestImage)
                                                                          .ExposePort(VoucherRedemptionMobileDockerHelper.VoucherManagementACLDockerPort)
                                                                          .UseNetwork(networkServices.ToArray()).Mount(hostFolder, "/home", MountType.ReadWrite);

            if (string.IsNullOrEmpty(hostFolder) == false)
            {
                voucherManagementAclContainer = voucherManagementAclContainer.Mount(hostFolder, "/home/txnproc/trace", MountType.ReadWrite);
            }

            if (dockerCredentials.HasValue)
            {
                voucherManagementAclContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = voucherManagementAclContainer.Build().Start()
                                                                            .WaitForPort($"{VoucherRedemptionMobileDockerHelper.VoucherManagementACLDockerPort}/tcp",
                                                                                         30000);

            logger.LogInformation("Voucher Management ACL Container Started");

            return builtContainer;
        }

        public static IContainerService SetupVoucherManagementContainer(String containerName,
                                                                        ILogger logger,
                                                                        String imageName,
                                                                        List<INetworkService> networkServices,
                                                                        String hostFolder,
                                                                        (String URL, String UserName, String Password)? dockerCredentials,
                                                                        String securityServiceContainerName,
                                                                        String estateManagementContainerName,
                                                                        String eventStoreContainerName,
                                                                        (String sqlServerContainerName, String sqlServerUserName, String sqlServerPassword)
                                                                            sqlServerDetails,
                                                                        (String clientId, String clientSecret) clientDetails,
                                                                        Boolean forceLatestImage = false,
                                                                        Int32 securityServicePort = VoucherRedemptionMobileDockerHelper.SecurityServiceDockerPort)
        {
            logger.LogInformation("About to Start Voucher Management Container");

            List<String> environmentVariables = new List<String>();
            environmentVariables
                .Add($"EventStoreSettings:ConnectionString=https://{eventStoreContainerName}:{VoucherRedemptionMobileDockerHelper.EventStoreHttpDockerPort}");
            environmentVariables.Add($"AppSettings:SecurityService=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables
                .Add($"AppSettings:EstateManagementApi=http://{estateManagementContainerName}:{VoucherRedemptionMobileDockerHelper.EstateManagementDockerPort}");
            environmentVariables.Add($"SecurityConfiguration:Authority=http://{securityServiceContainerName}:{securityServicePort}");
            environmentVariables.Add($"urls=http://*:{VoucherRedemptionMobileDockerHelper.VoucherManagementDockerPort}");
            environmentVariables.Add($"AppSettings:ClientId={clientDetails.clientId}");
            environmentVariables.Add($"AppSettings:ClientSecret={clientDetails.clientSecret}");
            environmentVariables
                .Add($"ConnectionStrings:EstateReportingReadModel=\"server={sqlServerDetails.sqlServerContainerName};user id={sqlServerDetails.sqlServerUserName};password={sqlServerDetails.sqlServerPassword};database=EstateReportingReadModel\"");

            ContainerBuilder voucherManagementContainer = new Builder().UseContainer().WithName(containerName).WithEnvironment(environmentVariables.ToArray())
                                                                       .UseImage(imageName, forceLatestImage)
                                                                       .ExposePort(VoucherRedemptionMobileDockerHelper.VoucherManagementDockerPort)
                                                                       .UseNetwork(networkServices.ToArray()).Mount(hostFolder, "/home", MountType.ReadWrite);

            if (string.IsNullOrEmpty(hostFolder) == false)
            {
                voucherManagementContainer = voucherManagementContainer.Mount(hostFolder, "/home/txnproc/trace", MountType.ReadWrite);
            }

            if (dockerCredentials.HasValue)
            {
                voucherManagementContainer.WithCredential(dockerCredentials.Value.URL, dockerCredentials.Value.UserName, dockerCredentials.Value.Password);
            }

            // Now build and return the container                
            IContainerService builtContainer = voucherManagementContainer.Build().Start();

            logger.LogInformation("Voucher Management  Container Started");

            return builtContainer;
        }

        public async Task StartContainersForScenarioRun(String scenarioName)
        {
            VoucherRedemptionMobileDockerHelper.LocalHostAddress = Environment.GetEnvironmentVariable("localhostaddress");
            if (string.IsNullOrEmpty(VoucherRedemptionMobileDockerHelper.LocalHostAddress))
            {
                VoucherRedemptionMobileDockerHelper.LocalHostAddress = "192.168.1.67";
            }

            this.Logger.LogInformation("In StartContainersForScenarioRun");
            String traceFolder = FdOs.IsWindows() ? $"D:\\home\\txnproc\\trace\\{scenarioName}" : $"//home//txnproc//trace//{scenarioName}";

            Logging.Enabled();

            Guid testGuid = Guid.NewGuid();
            this.TestId = testGuid;

            this.Logger.LogInformation($"Test Id is {testGuid}");

            // Setup the container names
            this.SecurityServiceContainerName = $"securityservice{testGuid:N}";
            this.EstateManagementContainerName = $"estate{testGuid:N}";
            this.EventStoreContainerName = $"eventstore{testGuid:N}";
            this.EstateReportingContainerName = $"estatereporting{testGuid:N}";
            this.SubscriptionServiceContainerName = $"subscription{testGuid:N}";
            this.MobileConfigurationContainerName = $"mobileConfig{this.TestId:N}";
            this.VoucherManagementContainerName = $"vouchermanagement{testGuid:N}";
            this.VoucherManagementACLContainerName = $"vouchermanagementacl{testGuid:N}";

            (String, String, String) dockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");

            INetworkService testNetwork = VoucherRedemptionMobileDockerHelper.SetupTestNetwork();
            this.TestNetworks.Add(testNetwork);

            IContainerService mobileConfigurationContainer =
                await VoucherRedemptionMobileDockerHelper.SetupMobileConfigurationContainer(this.MobileConfigurationContainerName,
                                                                                            this.Logger,
                                                                                            "clue/json-server",
                                                                                            testNetwork);

            // Start the Database Server here
            IContainerService databaseServerContainer = await VoucherRedemptionMobileDockerHelper
                                                              .StartSqlContainerWithOpenConnection(VoucherRedemptionMobileDockerHelper.SqlServerContainerName,
                                                                                                   this.Logger,
                                                                                                   "justin2004/mssql_server_tiny",
                                                                                                   testNetwork,
                                                                                                   "",
                                                                                                   dockerCredentials).ConfigureAwait(false);

            VoucherRedemptionMobileDockerHelper.DatabaseServerContainer = databaseServerContainer;

            IContainerService eventStoreContainer =
                await VoucherRedemptionMobileDockerHelper.SetupEventStoreContainer(this.EventStoreContainerName,
                                                                                   this.Logger,
                                                                                   "eventstore/eventstore:20.6.0-buster-slim",
                                                                                   testNetwork,
                                                                                   traceFolder,
                                                                                   usesEventStore2006OrLater:true);

            IContainerService estateManagementContainer = VoucherRedemptionMobileDockerHelper.SetupEstateManagementContainer(this.EstateManagementContainerName,
                this.Logger,
                "stuartferguson/estatemanagement",
                new List<INetworkService>
                {
                    testNetwork
                },
                traceFolder,
                null,
                this.SecurityServiceContainerName,
                this.EventStoreContainerName,
                VoucherRedemptionMobileDockerHelper.SqlServerContainerName,
                "sa",
                "thisisalongpassword123!",
                ("serviceClient", "Secret1"),
                true);

            IContainerService securityServiceContainer = VoucherRedemptionMobileDockerHelper.SetupSecurityServiceContainer(this.SecurityServiceContainerName,
                this.Logger,
                "stuartferguson/securityservice",
                testNetwork,
                traceFolder,
                dockerCredentials,
                true);

            IContainerService voucherManagementContainer = VoucherRedemptionMobileDockerHelper.SetupVoucherManagementContainer(this.VoucherManagementContainerName,
                this.Logger,
                "stuartferguson/vouchermanagement",
                new List<INetworkService>
                {
                    testNetwork
                },
                traceFolder,
                dockerCredentials,
                this.SecurityServiceContainerName,
                this.EstateManagementContainerName,
                this.EventStoreContainerName,
                (VoucherRedemptionMobileDockerHelper.SqlServerContainerName, VoucherRedemptionMobileDockerHelper.SqlUserName,
                    VoucherRedemptionMobileDockerHelper.SqlPassword),
                ("serviceClient", "Secret1"),
                true);

            IContainerService voucherManagementAclContainer =
                VoucherRedemptionMobileDockerHelper.SetupVoucherManagementACLContainer(this.VoucherManagementACLContainerName,
                                                                                       this.Logger,
                                                                                       "stuartferguson/vouchermanagementacl",
                                                                                       new List<INetworkService>
                                                                                       {
                                                                                           testNetwork
                                                                                       },
                                                                                       traceFolder,
                                                                                       dockerCredentials,
                                                                                       this.SecurityServiceContainerName,
                                                                                       this.VoucherManagementContainerName,
                                                                                       ("serviceClient", "Secret1"));

            IContainerService estateReportingContainer = VoucherRedemptionMobileDockerHelper.SetupEstateReportingContainer(this.EstateReportingContainerName,
                this.Logger,
                "stuartferguson/estatereporting",
                new List<INetworkService>
                {
                    testNetwork
                },
                traceFolder,
                dockerCredentials,
                this.SecurityServiceContainerName,
                VoucherRedemptionMobileDockerHelper.SqlServerContainerName,
                "sa",
                "thisisalongpassword123!",
                ("serviceClient", "Secret1"),
                true);

            this.Containers.AddRange(new List<IContainerService>
                                     {
                                         eventStoreContainer,
                                         estateManagementContainer,
                                         securityServiceContainer,
                                         estateReportingContainer,
                                         databaseServerContainer,
                                         mobileConfigurationContainer,
                                         voucherManagementContainer,
                                         voucherManagementAclContainer
                                     });

            // Cache the ports
            this.EstateManagementApiPort = estateManagementContainer.ToHostExposedEndpoint("5000/tcp").Port;
            this.SecurityServicePort = securityServiceContainer.ToHostExposedEndpoint("5001/tcp").Port;
            this.EventStoreHttpPort = eventStoreContainer.ToHostExposedEndpoint("2113/tcp").Port;
            this.VoucherManagementPort = voucherManagementContainer.ToHostExposedEndpoint("5007/tcp").Port;
            this.VoucherManagementACLPort = voucherManagementAclContainer.ToHostExposedEndpoint("5008/tcp").Port;
            this.MobileConfigurationContainerPort = mobileConfigurationContainer.ToHostExposedEndpoint("80/tcp").Port;

            String EstateManagementBaseAddressResolver(String api) => $"http://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{this.EstateManagementApiPort}";
            String SecurityServiceBaseAddressResolver(String api) => $"http://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{this.SecurityServicePort}";
            String VoucherManagementBaseAddressResolver(String api) => $"http://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{this.VoucherManagementPort}";

            String VoucherManagementAclBaseAddressResolver(String api) =>
                $"http://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{this.VoucherManagementACLPort}";

            String MobileConfigBaseAddressResolver(String api) =>
                $"http://{VoucherRedemptionMobileDockerHelper.LocalHostAddress}:{this.MobileConfigurationContainerPort}";

            this.SecurityServiceBaseAddress = SecurityServiceBaseAddressResolver(string.Empty);
            this.VoucherManagementBaseAddress = VoucherManagementBaseAddressResolver(string.Empty);
            this.VoucherManagementACLBaseAddress = VoucherManagementAclBaseAddressResolver(string.Empty);
            this.MobileConfigBaseAddress = MobileConfigBaseAddressResolver(string.Empty);

            HttpClient httpClient = new HttpClient();
            this.SecurityServiceClient = new SecurityServiceClient(SecurityServiceBaseAddressResolver, httpClient);
            this.EstateClient = new EstateClient(EstateManagementBaseAddressResolver, httpClient);
            this.VoucherManagementClient = new VoucherManagementClient(VoucherManagementBaseAddressResolver, httpClient);
            this.ACLHttpClient = new HttpClient();
            this.ACLHttpClient.BaseAddress = new Uri(VoucherManagementAclBaseAddressResolver(string.Empty));

            this.MobileConfigHttpClient = new HttpClient();

            await this.InitialiseAppConfig();

            await this.LoadEventStoreProjections().ConfigureAwait(false);

            await this.PopulateSubscriptionServiceConfiguration().ConfigureAwait(false);

            IContainerService subscriptionServiceContainer = VoucherRedemptionMobileDockerHelper.SetupSubscriptionServiceContainer(this.SubscriptionServiceContainerName,
                this.Logger,
                "stuartferguson/subscriptionservicehost",
                new List<INetworkService>
                {
                    testNetwork
                },
                traceFolder,
                dockerCredentials,
                this.SecurityServiceContainerName,
                VoucherRedemptionMobileDockerHelper.SqlServerContainerName,
                "sa",
                "thisisalongpassword123!",
                this.TestId,
                ("serviceClient", "Secret1"),
                true);

            this.Containers.Add(subscriptionServiceContainer);
        }

        public static async Task<IContainerService> StartSqlContainerWithOpenConnection(String containerName,
                                                                                        ILogger logger,
                                                                                        String imageName,
                                                                                        INetworkService networkService,
                                                                                        String hostFolder,
                                                                                        (String URL, String UserName, String Password)? dockerCredentials,
                                                                                        String sqlUserName = "sa",
                                                                                        String sqlPassword = "thisisalongpassword123!")
        {
            logger.LogInformation("About to start SQL Server Container");
            IContainerService databaseServerContainer = new Builder().UseContainer().WithName(containerName).UseImage(imageName)
                                                                     .WithEnvironment("ACCEPT_EULA=Y", $"SA_PASSWORD={sqlPassword}").ExposePort(1433)
                                                                     .UseNetwork(networkService).KeepContainer().KeepRunning().ReuseIfExists().Build().Start();

            logger.LogInformation("SQL Server Container Started");

            logger.LogInformation("About to SQL Server Container is running");
            IPEndPoint sqlServerEndpoint = null;
            await Retry.For(async () => { sqlServerEndpoint = databaseServerContainer.ToHostExposedEndpoint("1433/tcp"); }).ConfigureAwait(false);

            // Try opening a connection
            Int32 maxRetries = 10;
            Int32 counter = 1;

            String localhostaddress = Environment.GetEnvironmentVariable("localhostaddress");
            if (string.IsNullOrEmpty(localhostaddress))
            {
                localhostaddress = "192.168.1.67";
            }

            String server = localhostaddress;
            String database = "master";
            String user = sqlUserName;
            String password = sqlPassword;
            String port = sqlServerEndpoint.Port.ToString();

            String connectionString = $"server={server},{port};user id={user}; password={password}; database={database};";
            logger.LogInformation($"Connection String {connectionString}");
            SqlConnection connection = new SqlConnection(connectionString);
            Boolean databaseFound = false;
            while (counter <= maxRetries)
            {
                try
                {
                    logger.LogInformation($"Database Connection Attempt {counter}");

                    await connection.OpenAsync();

                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "select * from sys.databases";
                    SqlDataReader dataReader = await command.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).ConfigureAwait(false);

                    logger.LogInformation("Connection Opened");

                    // Check if we need to create the SS database
                    if (dataReader.HasRows)
                    {
                        while (await dataReader.ReadAsync(CancellationToken.None).ConfigureAwait(false))
                        {
                            if (await dataReader.GetFieldValueAsync<String>(0, CancellationToken.None).ConfigureAwait(false) == "SubscriptionServiceConfiguration")
                            {
                                databaseFound = true;
                                break;
                            }
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                    logger.LogInformation("SQL Server Container Running");
                    Console.WriteLine("SQL Server Container Running");
                    break;
                }
                catch(SqlException ex)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    logger.LogError(ex);
                    Thread.Sleep(20000);
                }
                finally
                {
                    counter++;
                }
            }

            if (databaseFound == false)
            {
                // Create the SS database here
                // Read the SQL File
                String sqlToExecute = null;
                String executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                String sqlFileLocation = Path.Combine(executableLocation, "DbScripts");
                IOrderedEnumerable<String> files = Directory.GetFiles(sqlFileLocation).OrderBy(x => x);

                try
                {
                    SqlConnection ssconnection = new SqlConnection(connectionString);
                    await ssconnection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                    SqlCommand sscommand = ssconnection.CreateCommand();
                    sscommand.CommandText = "CREATE DATABASE SubscriptionServiceConfiguration";
                    await sscommand.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);

                    sscommand.CommandText = "USE SubscriptionServiceConfiguration";
                    await sscommand.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);

                    foreach (String file in files)
                    {
                        using(StreamReader sr = new StreamReader(file))
                        {
                            sqlToExecute = sr.ReadToEnd();
                        }

                        sscommand.CommandText = sqlToExecute;
                        await sscommand.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                    }

                    connection.Close();

                    Console.WriteLine("SS Database Created");
                }
                catch(Exception e)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    Console.WriteLine(e);
                    throw;
                }
            }

            return databaseServerContainer;
        }

        public async Task StopContainersForScenarioRun()
        {
            await this.CleanUpSubscriptionServiceConfiguration().ConfigureAwait(false);

            if (this.Containers.Any())
            {
                foreach (IContainerService containerService in this.Containers)
                {
                    containerService.StopOnDispose = true;
                    containerService.RemoveOnDispose = true;
                    containerService.Dispose();
                }
            }

            if (this.TestNetworks.Any())
            {
                foreach (INetworkService networkService in this.TestNetworks)
                {
                    networkService.Stop();
                    networkService.Remove(true);
                }
            }
        }

        protected async Task CleanUpSubscriptionServiceConfiguration()
        {
            String connectionString = VoucherRedemptionMobileDockerHelper.GetLocalConnectionString("SubscriptionServiceConfiguration");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);

                // Delete the Event Store Server
                await this.DeleteEventStoreServer(connection).ConfigureAwait(false);

                // Delete the Subscriptions
                await this.DeleteSubscriptions(connection).ConfigureAwait(false);

                connection.Close();
            }
        }

        protected async Task DeleteEventStoreServer(SqlConnection openConnection)
        {
            SqlCommand command = openConnection.CreateCommand();
            command.CommandText = $"DELETE FROM EventStoreServer WHERE EventStoreServerId = '{this.TestId}'";
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task DeleteSubscriptions(SqlConnection openConnection)
        {
            SqlCommand command = openConnection.CreateCommand();
            command.CommandText = $"DELETE FROM Subscription WHERE EventStoreId = '{this.TestId}'";
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task InsertEventStoreServer(SqlConnection openConnection,
                                                    String eventStoreContainerName)
        {
            String esConnectionString =
                $"ConnectTo=tcp://admin:changeit@{eventStoreContainerName}:{VoucherRedemptionMobileDockerHelper.EventStoreTcpDockerPort};VerboseLogging=true;";
            SqlCommand command = openConnection.CreateCommand();
            command.CommandText =
                $"INSERT INTO EventStoreServer(EventStoreServerId, ConnectionString,Name) SELECT '{this.TestId}', '{esConnectionString}', 'TestEventStore'";
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task InsertSubscription(SqlConnection openConnection,
                                                String streamName,
                                                String groupName,
                                                String endPointUri)
        {
            SqlCommand command = openConnection.CreateCommand();
            command.CommandText =
                $"INSERT INTO subscription(SubscriptionId, EventStoreId, StreamName, GroupName, EndPointUri, StreamPosition) SELECT '{Guid.NewGuid()}', '{this.TestId}', '{streamName}', '{groupName}', '{endPointUri}', null";
            command.CommandType = CommandType.Text;
            await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected async Task PopulateSubscriptionServiceConfiguration()
        {
            String connectionString = VoucherRedemptionMobileDockerHelper.GetLocalConnectionString("SubscriptionServiceConfiguration");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);

                // Create an Event Store Server
                await this.InsertEventStoreServer(connection, this.EventStoreContainerName).ConfigureAwait(false);

                String reportingEndPointUri = $"http://{this.EstateReportingContainerName}:5005/api/domainevents";
                //String transactionProcessorEndPointUri = $"http://{this.TransactionProcessorContainerName}:5002/api/domainevents";

                // Add Route for Estate Aggregate Events
                await this.InsertSubscription(connection, "$ce-EstateAggregate", "Reporting", reportingEndPointUri).ConfigureAwait(false);

                // Add Route for Merchant Aggregate Events
                await this.InsertSubscription(connection, "$ce-MerchantAggregate", "Reporting", reportingEndPointUri).ConfigureAwait(false);

                // Add Route for Contract Aggregate Events
                await this.InsertSubscription(connection, "$ce-ContractAggregate", "Reporting", reportingEndPointUri).ConfigureAwait(false);

                // Add Route for Voucher Aggregate Events
                await this.InsertSubscription(connection, "$ce-VoucherAggregate", "Reporting", reportingEndPointUri).ConfigureAwait(false);

                connection.Close();
            }
        }

        private async Task InitialiseAppConfig()
        {
            DevelopmentConfiguration config = new DevelopmentConfiguration();
            config.EnableAutoUpdates = false;
            config.ClientId = "redemptionClient";
            config.ClientSecret = "Secret1";
            config.SecurityService = this.SecurityServiceBaseAddress;
            config.VoucherManagementACL = this.VoucherManagementACLBaseAddress;
            config.DeviceIdentifier = AppManager.GetDeviceIdentifier();
            config.id = AppManager.GetDeviceIdentifier();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{this.MobileConfigBaseAddress}/voucherconfiguration");
            request.Content = new StringContent(JsonConvert.SerializeObject(config), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await this.MobileConfigHttpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        private async Task LoadEventStoreProjections()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            //Start our Continous Projections - we might decide to do this at a different stage, but now lets try here
            String projectionsFolder = "projections/continuous";
            IPAddress[] ipAddresses = Dns.GetHostAddresses(VoucherRedemptionMobileDockerHelper.LocalHostAddress);
            IPEndPoint endpoint = new IPEndPoint(ipAddresses.First(), this.EventStoreHttpPort);

            if (!string.IsNullOrWhiteSpace(projectionsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                di = di.Parent.Parent;

                DirectoryInfo projectionFolder = new DirectoryInfo($"{di.FullName}/{projectionsFolder}");

                if (projectionFolder.Exists)
                {
                    FileInfo[] files = projectionFolder.GetFiles();

                    EventStoreHttp projectionManager = new EventStoreHttp(endpoint);

                    foreach (FileInfo file in files)
                    {
                        String projection = File.ReadAllText(file.FullName);
                        String projectionName = file.Name.Replace(".js", string.Empty);

                        try
                        {
                            this.Logger.LogInformation($"Creating projection [{projectionName}]");
                            await projectionManager.CreateContinuousProjection(projectionName, projection, true, "admin", "changeit", CancellationToken.None);
                        }
                        catch(Exception e)
                        {
                            this.Logger.LogError(new Exception($"Projection [{projectionName}] error", e));
                        }
                    }
                }
            }

            this.Logger.LogInformation("Loaded projections");
        }

        private async Task RemoveEstateReadModel()
        {
            //List<Guid> estateIdList = this.TestingContext.GetAllEstateIds();

            //foreach (Guid estateId in estateIdList)
            //{
            //    String databaseName = $"EstateReportingReadModel{estateId}";

            //    await Retry.For(async () =>
            //    {
            //        // Build the connection string (to master)
            //        String connectionString = VoucherRedemptionMobileDockerHelper.GetLocalConnectionString(databaseName);
            //        EstateReportingContext context = new EstateReportingContext(connectionString);
            //        await context.Database.EnsureDeletedAsync(CancellationToken.None);
            //    });
            //}
        }

        #endregion

        #region Others

        public const Int32 EstateManagementDockerPort = 5000;

        public const Int32 EstateReportingDockerPort = 5005;

        /// <summary>
        /// The event store HTTP docker port
        /// </summary>
        public const Int32 EventStoreHttpDockerPort = 2113;

        /// <summary>
        /// The event store TCP docker port
        /// </summary>
        public const Int32 EventStoreTcpDockerPort = 1113;

        public const Int32 SecurityServiceDockerPort = 5001;

        public const String SqlPassword = "thisisalongpassword123!";

        public const String SqlUserName = "sa";

        public const Int32 VoucherManagementACLDockerPort = 5008;

        public const Int32 VoucherManagementDockerPort = 5007;

        #endregion
    }

    public class DevelopmentConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public String ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public String ClientSecret { get; set; }

        public String DeviceIdentifier { get; set; }

        public Boolean EnableAutoUpdates { get; set; }

        public String id { get; set; }

        /// <summary>
        /// Gets or sets the security service.
        /// </summary>
        /// <value>
        /// The security service.
        /// </value>
        [JsonProperty("securityServiceUri")]
        public String SecurityService { get; set; }

        /// <summary>
        /// Gets or sets the transaction processor acl.
        /// </summary>
        /// <value>
        /// The transaction processor acl.
        /// </value>
        [JsonProperty("voucherManagementACLUri")]
        public String VoucherManagementACL { get; set; }

        #endregion
    }
}