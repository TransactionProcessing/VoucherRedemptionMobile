namespace VoucherRedemptionMobile.iOS
{
    using System;
    using System.Threading.Tasks;
    using Foundation;
    using Syncfusion.XForms.iOS.Border;
    using Syncfusion.XForms.iOS.Buttons;
    using Syncfusion.XForms.iOS.TabView;
    using UIKit;
    using VoucherRedemptionMobile;
    using VoucherRedemptionMobile.Common;
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
        /// The configuration
        /// </summary>
        private IConfiguration Configuration;

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

            this.Device = new iOSDevice();
            //this.AnalysisLogger = new AppCenterAnalysisLogger();

            Forms.Init();

            Calabash.Start();

            SfBorderRenderer.Init();
            SfButtonRenderer.Init();
            SfTabViewRenderer.Init();

            // TODO: fix this
            //this.LoadApplication(new App(this.Device, this.AnalysisLogger));

            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        [Export("SetConfiguration:")]
        public void SetConfiguration(NSString configuration)
        {
            //String[] configItems = configuration.ToString().Split(',');
            //Configuration configurationObject = new Configuration
            //                                    {
            //                                        ClientId = configItems[0],
            //                                        ClientSecret = configItems[1],
            //                                        SecurityService = configItems[2],
            //                                        TransactionProcessorACL = configItems[3],
            //                                        EstateManagement = configItems[4]
            //};

            //App.Configuration = configurationObject;
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