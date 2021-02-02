namespace VoucherRedemptionMobile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Database;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Distribute;
    using Plugin.Toast;
    using Plugin.Toast.Abstractions;
    using Presenters;
    using SecurityService.DataTransferObjects.Responses;
    using StructureMap;
    using Syncfusion.Licensing;
    using VoucherRedemption.Clients;
    using Xamarin.Forms;
    using IDevice = Common.IDevice;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Application" />
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        #region Fields

        /// <summary>
        /// The configuration
        /// </summary>
        public static IConfiguration Configuration;

        public static Boolean IsIntegrationTestMode;
        /// <summary>
        /// Unity container
        /// </summary>
        public static IContainer Container;

        /// <summary>
        /// The token response
        /// </summary>
        public static TokenResponse TokenResponse;

        /// <summary>
        /// The estate identifier
        /// </summary>
        public static Guid EstateId;

        /// <summary>
        /// The merchant identifier
        /// </summary>
        public static Guid MerchantId;

        /// <summary>
        /// The transaction number
        /// </summary>
        private static Int32 TransactionNumber;

        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// The device
        /// </summary>
        private readonly IDevice Device;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="database">The database.</param>
        public App(IDevice device,
                   IDatabaseContext database)
        {
            this.Device = device;
            this.Database = database;

            // Init the logging DB
            this.Database.InitialiseDatabase();

            this.InitializeComponent();

            SyncfusionLicenseProvider.RegisterLicense("MzcxNTgwQDMxMzgyZTM0MmUzMEpNaXFsaFQ5bFErcVord05LSkVUeVd0MWRyaVE0NllvcFJmRzVZTWVTNlE9");

            App.Container = Bootstrapper.Run();

            App.Container.Configure((c) =>
                                    {
                                        c.For<IDevice>().Use(device).Transient();
                                        c.For<IDatabaseContext>().Use(database).Transient();
                                    });

            App.Container.AssertConfigurationIsValid();

            if (App.Configuration == null)
            {
                Task.Run(async () =>
                         {
                             try
                             {
                                 // TODO: Logging
                                 Console.WriteLine("Config is null");
                                 IConfigurationServiceClient configurationServiceClient = App.Container.GetInstance<IConfigurationServiceClient>();
                                 App.Configuration = await configurationServiceClient.GetConfiguration(this.Device.GetDeviceIdentifier(), CancellationToken.None);
                                 // TODO: Logging
                                 Console.WriteLine("Config retrieved");
                             }
                             catch(Exception ex)
                             {
                                 CrossToastPopUp.Current.ShowToastWarning("Error retrieving configuration.", ToastLength.Long);
                             }
                         }).Wait();

            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Application developers override this method to perform actions when the application resumes from a sleeping state.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application enters the sleeping state.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application starts.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override async void OnStart()
        {
            // TODO: Logging
            Console.WriteLine("In On Start");

            if (App.Configuration != null)
            {
                if (App.Configuration.EnableAutoUpdates)
                {
                    Distribute.SetEnabledAsync(true).Wait();
                    Distribute.CheckForUpdate();
                }
                else
                {
                    Distribute.DisableAutomaticCheckForUpdate();
                }

                Distribute.ReleaseAvailable = this.OnReleaseAvailable;
                Distribute.UpdateTrack = UpdateTrack.Public;

                AppCenter.Start("android=10210e06-8a11-422b-b005-14081dc56375;", typeof(Distribute));
            }

            App.TransactionNumber = 1;

            // Handle when your app starts
            ILoginPresenter loginPresenter = App.Container.GetInstance<ILoginPresenter>();
            
            // show the login page
            await loginPresenter.Start();
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user can't postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = App.Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Later");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // This method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            // Return true if you're using your own dialog, false otherwise
            return true;
        }

        #endregion
    }
}