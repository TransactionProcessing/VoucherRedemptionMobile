namespace VoucherRedemptionMobile.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using EstateManagement.Client;
    using Pages;
    using Presenters;
    using SecurityService.Client;
    using TestClients;
    using Unity;
    using Unity.Lifetime;
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
        public static IUnityContainer Run()
        {
            IUnityContainer container = new UnityContainer();
            
            Bootstrapper.RegisterClients(container);
            Bootstrapper.RegisterPresenters(container);
            Bootstrapper.RegisterViews(container);
            Bootstrapper.RegisterViewModels(container);
            
            return container;
        }

        private static void RegisterClients(IUnityContainer container)
        {
            if (App.IsIntegrationTestMode)
            {
                container.RegisterSingleton<IConfigurationServiceClient, TestConfigurationServiceClient>();
                container.RegisterSingleton<ISecurityServiceClient, TestSecurityServiceClient>();
                container.RegisterSingleton<IVoucherManagerACLClient, TestVoucherManagementACLClient>();
            }
            else
            {
                container.RegisterSingleton<IConfigurationServiceClient, ConfigurationServiceClient>();
                container.RegisterSingleton<ISecurityServiceClient, SecurityServiceClient>();
                container.RegisterSingleton<IVoucherManagerACLClient, VoucherManagerACLClient>();
                HttpClientHandler httpClientHandler = new HttpClientHandler
                                                      {
                                                          ServerCertificateCustomValidationCallback = (message,
                                                                                                       certificate2,
                                                                                                       arg3,
                                                                                                       arg4) =>
                                                                                                      {
                                                                                                          return true;
                                                                                                      }
                                                      };
                HttpClient httpClient = new HttpClient(httpClientHandler);
                container.RegisterInstance(httpClient);
                container.RegisterInstance<Func<String, String>>(
                new Func<String, String>(configSetting =>
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

        private static void RegisterPresenters(IUnityContainer container)
        {
            container.RegisterType<ILoginPresenter, LoginPresenter>(new TransientLifetimeManager());
            container.RegisterType<ISupportPresenter, SupportPresenter>(new TransientLifetimeManager());
            container.RegisterType<IVoucherPresenter, VoucherPresenter>(new TransientLifetimeManager());
        }

        private static void RegisterViews(IUnityContainer container)
        {
            // General
            container.RegisterType<IMainPage, MainPage>(new TransientLifetimeManager());
            container.RegisterType<ILoginPage, LoginPage>(new TransientLifetimeManager());

            // Support
            container.RegisterType<ISupportPage, SupportPage>(new TransientLifetimeManager());

            // Voucher
            container.RegisterType<IVoucherPage, VoucherPage>(new TransientLifetimeManager());
            container.RegisterType<IRedemptionEnterVoucherCodePage, RedemptionEnterVoucherCodePage>(new TransientLifetimeManager());
            container.RegisterType<IRedemptionVoucherDetailsPage, RedemptionVoucherDetailsPage>(new TransientLifetimeManager());
            container.RegisterType<IRedemptionSuccessPage, RedemptionSuccessPage>(new TransientLifetimeManager());
            container.RegisterType<IRedemptionFailedPage, RedemptionFailedPage>(new TransientLifetimeManager());
        }

        private static void RegisterViewModels(IUnityContainer container)
        {
            container.RegisterType<LoginViewModel>(new TransientLifetimeManager());
            container.RegisterType<MainPageViewModel>(new TransientLifetimeManager());
            container.RegisterType<RedemptionEnterVoucherCodeViewModel>(new TransientLifetimeManager());
            container.RegisterType<RedemptionEnterVoucherCodeViewModel>(new TransientLifetimeManager());
        }

        #endregion
    }
}