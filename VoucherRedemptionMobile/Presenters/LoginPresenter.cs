namespace VoucherRedemptionMobile.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
    using TransactionProcessorACL.DataTransferObjects;
    using TransactionProcessorACL.DataTransferObjects.Responses;
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

        private readonly ISupportPage SupportPage;

        /// <summary>
        /// The main page view model
        /// </summary>
        private readonly MainPageViewModel MainPageViewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPresenter" /> class.
        /// </summary>
        /// <param name="loginPage">The login page.</param>
        /// <param name="mainPage">The main page.</param>
        /// <param name="supportPage">The support page.</param>
        /// <param name="loginViewModel">The login view model.</param>
        /// <param name="mainPageViewModel">The main page view model.</param>
        /// <param name="device">The device.</param>
        /// <param name="database">The logging database.</param>
        public LoginPresenter(ILoginPage loginPage,
                              IMainPage mainPage,
                              ISupportPage supportPage,
                              LoginViewModel loginViewModel,
                              MainPageViewModel mainPageViewModel,
                              IDevice device,
                              IDatabaseContext database)
        {
            this.MainPage = mainPage;
            this.SupportPage = supportPage;
            this.LoginPage = loginPage;
            this.LoginViewModel = loginViewModel;
            this.MainPageViewModel = mainPageViewModel;
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
            ISupportPresenter supportPresenter = App.Container.GetInstance<ISupportPresenter>();
            supportPresenter.Start();
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
                IConfigurationServiceClient configurationServiceClient = App.Container.GetInstance<IConfigurationServiceClient>();
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
                ISecurityServiceClient securityServiceClient = App.Container.GetInstance<ISecurityServiceClient>();
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
;
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastWarning("Incorrect username or password entered, please try again!");
                }
            }
        }

        private void MainPage_VoucherButtonClicked(object sender, EventArgs e)
        {
            IVoucherPresenter voucherPresenter = App.Container.GetInstance<IVoucherPresenter>();
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
            ISupportPresenter supportPresenter = App.Container.GetInstance<ISupportPresenter>();
            supportPresenter.Start();
        }
        
        #endregion
    }
}