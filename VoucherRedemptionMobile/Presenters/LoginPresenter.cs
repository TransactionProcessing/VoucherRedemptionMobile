﻿namespace VoucherRedemptionMobile.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Common;
    using Database;
    using EstateManagement.Client;
    using EstateManagement.DataTransferObjects.Responses;
    using Newtonsoft.Json;
    using Pages;
    using Plugin.Toast;
    using Plugin.Toast.Abstractions;
    using SecurityService.Client;
    using SecurityService.DataTransferObjects.Responses;
    using TestClients;
    using TestClients.Models;
    using Unity;
    using Unity.Lifetime;
    using ViewModels;
    using VoucherRedemption.Clients;
    using Xamarin.Forms;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ILoginPresenter" />
    [ExcludeFromCodeCoverage]
    public class LoginPresenter : ILoginPresenter
    {
        #region Fields

        /// <summary>
        /// The device
        /// </summary>
        private readonly IDevice Device;

        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// The login page
        /// </summary>
        private readonly ILoginPage LoginPage;

        /// <summary>
        /// The login view model
        /// </summary>
        private readonly LoginViewModel LoginViewModel;

        /// <summary>
        /// The main page
        /// </summary>
        private readonly IMainPage MainPage;

        private readonly ITestModePage TestModePage;

        private readonly ISupportPage SupportPage;

        /// <summary>
        /// The main page view model
        /// </summary>
        private readonly MainPageViewModel MainPageViewModel;

        private readonly TestModePageViewModel TestModePageViewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPresenter" /> class.
        /// </summary>
        /// <param name="loginPage">The login page.</param>
        /// <param name="mainPage">The main page.</param>
        /// <param name="testModePage">The test mode page.</param>
        /// <param name="supportPage">The support page.</param>
        /// <param name="loginViewModel">The login view model.</param>
        /// <param name="mainPageViewModel">The main page view model.</param>
        /// <param name="testModePageViewModel">The test mode page view model.</param>
        /// <param name="device">The device.</param>
        /// <param name="database">The logging database.</param>
        public LoginPresenter(ILoginPage loginPage,
                              IMainPage mainPage,
                              ITestModePage testModePage,
                              ISupportPage supportPage,
                              LoginViewModel loginViewModel,
                              MainPageViewModel mainPageViewModel,
                              TestModePageViewModel testModePageViewModel,
                              IDevice device,
                              IDatabaseContext database)
        {
            this.MainPage = mainPage;
            this.TestModePage = testModePage;
            this.SupportPage = supportPage;
            this.LoginPage = loginPage;
            this.LoginViewModel = loginViewModel;
            this.MainPageViewModel = mainPageViewModel;
            this.TestModePageViewModel = testModePageViewModel;
            this.Device = device;
            this.Database = database;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public async Task Start()
        {
            await this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage("In Start"));
            
            this.LoginPage.LoginButtonClick += this.LoginPage_LoginButtonClick;
            this.LoginPage.SupportButtonClick += this.LoginPage_SupportButtonClick;
            this.LoginPage.TestModeButtonClick += this.LoginPage_TestModeButtonClick;

            this.LoginPage.Init(this.LoginViewModel);

            Application.Current.MainPage = new NavigationPage((Page)this.LoginPage);
        }

        /// <summary>
        /// Handles the SupportButtonClick event of the LoginPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoginPage_SupportButtonClick(object sender, EventArgs e)
        {
            ISupportPresenter supportPresenter = App.Container.Resolve<ISupportPresenter>();
            supportPresenter.Start();
        }

        private async void TestModePage_SetTestModeButtonClick(object sender, EventArgs e)
        {
            // TODO: Validate Pin
            // Set app in Test mode here
            App.IsIntegrationTestMode = true;
            App.Container = Bootstrapper.Run();

            IDatabaseContext database = new DatabaseContext(String.Empty);
            IDevice device = new TestDevice();
            App.Container.RegisterInstance(typeof(IDatabaseContext), database, new ContainerControlledLifetimeManager());
            App.Container.RegisterInstance(typeof(IDevice), device, new ContainerControlledLifetimeManager());

            // Read the test data
            var testVoucherData = this.TestModePageViewModel.TestVoucherData;
            var testUserData = this.TestModePageViewModel.TestUserData;
            UpdateTestVoucherData(testVoucherData);

            CrossToastPopUp.Current.ShowToastMessage(testUserData.Length.ToString());
            UpdateTestUserData(testUserData);

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void UpdateTestUserData(String userData)
        {
            List<(String,String)> users = JsonConvert.DeserializeObject<List< (String,String)>>(userData);
            if (users.Any())
            {
                TestSecurityServiceClient securityServiceClient = App.Container.Resolve<ISecurityServiceClient>() as TestSecurityServiceClient;
                foreach (var user in users)
                {
                    securityServiceClient.CreateUserDetails(user.Item1, user.Item2);
                }
            }
        }

        private void UpdateTestVoucherData(String voucherData)
        {
            List<Voucher> vouchers = JsonConvert.DeserializeObject<List<Voucher>>(voucherData);
            if (vouchers.Any())
            {
                TestVoucherManagementACLClient voucherManagementACLClient = App.Container.Resolve<IVoucherManagerACLClient>() as TestVoucherManagementACLClient;
                foreach (Voucher voucher in vouchers)
                {
                    voucherManagementACLClient.CreateTestVoucher(voucher.VoucherCode, voucher.Value, voucher.RecipientEmail, voucher.RecipientMobile);
                }
            }
        }

        private async void LoginPage_TestModeButtonClick(object sender, EventArgs e)
        {
            // Show the test mode page

            this.TestModePage.SetTestModeButtonClick += TestModePage_SetTestModeButtonClick;

            this.TestModePage.Init(this.TestModePageViewModel);
            await Application.Current.MainPage.Navigation.PushAsync((Page)this.TestModePage);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        private async Task GetConfiguration()
        {
            // Get the application configuration here
            try
            {
                Console.WriteLine("Config is null");
                IConfigurationServiceClient configurationServiceClient = App.Container.Resolve<IConfigurationServiceClient>();
                App.Configuration = await configurationServiceClient.GetConfiguration(this.Device.GetDeviceIdentifier(), CancellationToken.None);
                // TODO: Logging
                Console.WriteLine("Config retrieved");

                String config = JsonConvert.SerializeObject(App.Configuration);
                await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage(config));
            }
            catch (Exception ex)
            {
                // TODO: Handle this scenario better on CI :|
                throw new ApplicationException("Error getting configuration for device!");
            }
        }

        /// <summary>
        /// Handles the LoginButtonClick event of the LoginPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void LoginPage_LoginButtonClick(Object sender,
                                                      EventArgs e)
        {
            try
            {
                ISecurityServiceClient securityServiceClient = App.Container.Resolve<ISecurityServiceClient>();
                //this.LoginViewModel.EmailAddress = "redemptionuser@healthcarecentre1.co.uk";
                //this.LoginViewModel.Password = "123456";

                await this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage("About to Get Configuration"));
                await this.GetConfiguration();

                await
                    this.Database.InsertLogMessage(DatabaseContext
                                                       .CreateDebugLogMessage($"About to Get Token for User [{this.LoginViewModel.EmailAddress} with Password [{this.LoginViewModel.Password}]]"));


                // Attempt to login with the user details
                TokenResponse tokenResponse = await securityServiceClient.GetToken(this.LoginViewModel.EmailAddress,
                                                                                   this.LoginViewModel.Password,
                                                                                   App.Configuration.ClientId,
                                                                                   App.Configuration.ClientSecret,
                                                                                   CancellationToken.None);

                await this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"About to Cache Token {tokenResponse.AccessToken}"));

                // Cache the user token
                App.TokenResponse = tokenResponse;

                // Go to signed in page
                this.MainPage.Init(this.MainPageViewModel);
                this.MainPage.VoucherButtonClicked += this.MainPage_VoucherButtonClicked;
                this.MainPage.SupportButtonClicked += this.MainPage_SupportButtonClicked;

                Application.Current.MainPage = new NavigationPage((Page)this.MainPage);
            }
            catch(ApplicationException aex)
            {
                await this.Database.InsertLogMessages(DatabaseContext.CreateErrorLogMessages(aex));
                CrossToastPopUp.Current.ShowToastWarning(aex.Message);
            }
            catch(Exception ex)
            {
                await this.Database.InsertLogMessages(DatabaseContext.CreateErrorLogMessages(ex));

                if (ex.InnerException != null && ex.InnerException is ApplicationException)
                {
                    // Application needs to be upgraded to latest version
                    CrossToastPopUp.Current.ShowToastError("Application version is incompatible, please upgrade to the latest version!!",ToastLength.Long);
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastWarning("Incorrect username or password entered, please try again!");
                }
            }
        }

        private void MainPage_VoucherButtonClicked(object sender, EventArgs e)
        {
            IVoucherPresenter voucherPresenter = App.Container.Resolve<IVoucherPresenter>();
            voucherPresenter.Start();
        }

        /// <summary>
        /// Handles the SupportButtonClicked event of the MainPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void MainPage_SupportButtonClicked(Object sender,
                                                   EventArgs e)
        {
            ISupportPresenter supportPresenter = App.Container.Resolve<ISupportPresenter>();
            supportPresenter.Start();
        }
        
        #endregion
    }
}