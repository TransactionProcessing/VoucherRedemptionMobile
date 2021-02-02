namespace VoucherRedemptionMobile.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using EstateManagement.Client;
    using Pages;
    using Presenters;
    using SecurityService.Client;
    using StructureMap;
    using TestClients;
    using ViewModels;
    using Views;
    using Views.Redemption;
    using VoucherRedemption.Clients;

    [ExcludeFromCodeCoverage]
    public class Bootstrapper
    {
        #region Methods

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        public static IContainer Run()
        {
            Registry registry = new Registry();
            registry.IncludeRegistry<ClientsRegistry>();
            registry.IncludeRegistry<PresenterRegistry>();
            registry.IncludeRegistry<ViewRegistry>();
            registry.IncludeRegistry<ViewModelRegistry>();
            IContainer container = new Container(registry);
            
            return container;
        }

        #endregion
    }

    public class ClientsRegistry : Registry
    {
        public ClientsRegistry()
        {
            Console.WriteLine($"App.IsIntegrationTestMode is {App.IsIntegrationTestMode}");

            if (App.IsIntegrationTestMode)
            {
                this.For<IConfigurationServiceClient>().Use<TestConfigurationServiceClient>().Singleton();
                this.For<ISecurityServiceClient>().Use<TestSecurityServiceClient>().Singleton();
                this.For<IVoucherManagerACLClient>().Use<TestVoucherManagementACLClient>().Singleton();
            }
            else
            {
                this.For<ISecurityServiceClient>().Use<SecurityServiceClient>().Singleton();
                this.For<IConfigurationServiceClient>().Use<ConfigurationServiceClient>().Singleton();
                this.For<IVoucherManagerACLClient>().Use<VoucherManagerACLClient>().Singleton();

                this.For<HttpClient>().Add(new HttpClient());
                this.For<Func<String, String>>().Add(new Func<String, String>(configSetting =>
                {
                    if (configSetting == "ConfigServiceUrl")
                    {
                        return "https://5r8nmm.deta.dev";
                    }

                    if (App.Configuration != null)
                    {
                        IConfiguration config = App.Configuration;

                        if (configSetting == "SecurityService")
                        {
                            return config.SecurityService;
                        }

                        if (configSetting == "VoucherManagementACL")
                        {
                            return config.VoucherManagementACL;
                        }

                        return string.Empty;
                    }

                    return string.Empty;
                }));
            }
        }
    }

    public class PresenterRegistry : Registry
    {
        public PresenterRegistry()
        {
            this.For<ILoginPresenter>().Add<LoginPresenter>().Transient();
            this.For<ISupportPresenter>().Add<SupportPresenter>().Transient();
            this.For<IVoucherPresenter>().Add<VoucherPresenter>().Transient();
        }
    }
    public class ViewRegistry : Registry
    {
        public ViewRegistry()
        {
            // General
            this.For<IMainPage>().Use<MainPage>().Transient();
            this.For<ILoginPage>().Use<LoginPage>().Transient();

            // Support
            this.For<ISupportPage>().Use<SupportPage>().Transient();

            // Voucher
            this.For<IVoucherPage>().Use<VoucherPage>().Transient();
            this.For<IRedemptionEnterVoucherCodePage>().Use<RedemptionEnterVoucherCodePage>().Transient();
            this.For<IRedemptionVoucherDetailsPage>().Use<RedemptionVoucherDetailsPage>().Transient();
            this.For<IRedemptionSuccessPage>().Use<RedemptionSuccessPage>().Transient();
            this.For<IRedemptionFailedPage>().Use<RedemptionFailedPage>().Transient();
        }
    }

    public class ViewModelRegistry : Registry
    {
        public ViewModelRegistry()
        {
            // General
            this.For<LoginViewModel>().Transient();
            this.For<MainPageViewModel>().Transient();
            this.For<RedemptionEnterVoucherCodeViewModel>().Transient();
            this.For<RedemptionVoucherDetailsViewModel>().Transient();
        }
    }
}