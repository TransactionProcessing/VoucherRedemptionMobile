namespace VoucherRedemptionMobile.iOS
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Database;
    using EstateManagement.Client;
    using Foundation;
    using Newtonsoft.Json;
    using SecurityService.Client;
    using Syncfusion.XForms.iOS.Border;
    using Syncfusion.XForms.iOS.Buttons;
    using Syncfusion.XForms.iOS.TabView;
    using TestClients;
    using UIKit;
    using Unity;
    using Unity.Lifetime;
    using VoucherRedemption.Clients;
    using VoucherRedemptionMobile;
    using VoucherRedemptionMobile.Common;
    using VoucherRedemptionMobile.TestClients.Models;
    using Xamarin;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.iOS;

    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.iOS.FormsApplicationDelegate" />
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        #region Fields

        /// <summary>
        /// The logging database
        /// </summary>
        private IDatabaseContext Database;

        /// <summary>
        /// The device
        /// </summary>
        private IDevice Device;

        #endregion

        #region Methods

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        /// <summary>
        /// Finisheds the launching.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public override Boolean FinishedLaunching(UIApplication app,
                                                  NSDictionary options)
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += this.TaskSchedulerOnUnobservedTaskException;

            //String connectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TransactionProcessing.db");
            this.Device = new iOSDevice();
            //this.Database = new DatabaseContext(connectionString);
            this.Database = new DatabaseContext();

            Forms.Init();

            Calabash.Start();

            SfBorderRenderer.Init();
            SfButtonRenderer.Init();
            SfTabViewRenderer.Init();

            this.LoadApplication(new App(this.Device, this.Database));

            return base.FinishedLaunching(app, options);
        }

        [Export("SetIntegrationTestModeOn:")]
        public void SetIntegrationTestModeOn(NSString input)
        {
            Console.WriteLine($"Inside SetIntegrationTestModeOn");
            App.IsIntegrationTestMode = true;
            //App.Container.Configure((c) =>
            //                        {
            //                            c.For<IConfigurationServiceClient>().ClearAll();
            //                            c.For<ISecurityServiceClient>().ClearAll();
            //                            c.For<IEstateClient>().ClearAll();
            //                            c.For<IVoucherManagerACLClient>().ClearAll();
            //                        });
            App.Container = Bootstrapper.Run();

            IDevice device = new iOSDevice();
            String connectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TransactionProcessing.db");
            DatabaseContext database = new DatabaseContext(connectionString);
            //App.Container.Configure((c) =>
            //                        {
            //                            c.For<IDevice>().Use(device).Transient();
            //                            c.For<IDatabaseContext>().Use(database).Transient();
            //                        });
            App.Container.RegisterInstance(this.Database, new ContainerControlledLifetimeManager());
            App.Container.RegisterInstance(this.Device, new ContainerControlledLifetimeManager());
        }

        [Export("AddTestVoucher:")]
        public void AddTestVoucher(NSString voucherData)
        {
            if (App.IsIntegrationTestMode == true)
            {
                Voucher voucher = JsonConvert.DeserializeObject<Voucher>(voucherData);
                TestVoucherManagementACLClient voucherManagerAclClient = App.Container.Resolve<IVoucherManagerACLClient>() as TestVoucherManagementACLClient;

                voucherManagerAclClient.Vouchers.Add(voucher);
            }
        }

        /// <summary>
        /// Currents the domain on unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="unhandledExceptionEventArgs">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomainOnUnhandledException(Object sender,
                                                       UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            // TODO: Logging
            //this.AnalysisLogger.TrackCrash(newExc);
        }

        /// <summary>
        /// Tasks the scheduler on unobserved task exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="unobservedTaskExceptionEventArgs">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        private void TaskSchedulerOnUnobservedTaskException(Object sender,
                                                            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            // TODO: Logging
            // this.AnalysisLogger.TrackCrash(newExc);
        }

        #endregion
    }
}