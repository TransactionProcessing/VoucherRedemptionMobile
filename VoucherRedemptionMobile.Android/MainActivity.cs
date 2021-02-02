namespace VoucherRedemptionMobile.Droid
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;
    using Common;
    using Database;
    using EstateManagement.Client;
    using Java.Interop;
    using Microsoft.AppCenter.Distribute;
    using Newtonsoft.Json;
    using SecurityService.Client;
    using TestClients;
    using TestClients.Models;
    using VoucherRedemption.Clients;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;
    using Environment = System.Environment;
    using Platform = Xamarin.Essentials.Platform;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.Android.FormsAppCompatActivity" />
    [Activity(Label = "VoucherRedemptionMobile",
              Icon = "@mipmap/icon",
              Theme = "@style/MainTheme",
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        #region Fields
                
        /// <summary>
        /// The device
        /// </summary>
        private IDevice Device;

        /// <summary>
        /// The logging database
        /// </summary>
        private IDatabaseContext Database;

        #endregion

        public MainActivity()
        {
            
        }

        #region Methods

        /// <summary>
        /// Called when [request permissions result].
        /// </summary>
        /// <param name="requestCode">The request code.</param>
        /// <param name="permissions">The permissions.</param>
        /// <param name="grantResults">The grant results.</param>
        /// <remarks>
        /// Portions of this page are modifications based on work created and shared by the <format type="text/html"><a href="https://developers.google.com/terms/site-policies" title="Android Open Source Project">Android Open Source Project</a></format> and used according to terms described in the <format type="text/html"><a href="https://creativecommons.org/licenses/by/2.5/" title="Creative Commons 2.5 Attribution License">Creative Commons 2.5 Attribution License.</a></format>
        /// </remarks>
        public override void OnRequestPermissionsResult(Int32 requestCode,
                                                        String[] permissions,
                                                        [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        [Export("SetIntegrationTestModeOn")]
        public void SetIntegrationTestModeOn()
        {
            Console.WriteLine($"Inside SetIntegrationTestModeOn");
            App.IsIntegrationTestMode = true;
            App.Container.Configure((c) =>
                                    {
                                        c.For<IConfigurationServiceClient>().ClearAll();
                                        c.For<ISecurityServiceClient>().ClearAll();
                                        c.For<IEstateClient>().ClearAll();
                                        c.For<IVoucherManagerACLClient>().ClearAll();
                                    });
            App.Container = Bootstrapper.Run();

            IDevice device = new AndroidDevice();
            String connectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TransactionProcessing.db");
            DatabaseContext database = new DatabaseContext(connectionString);
            App.Container.Configure((c) =>
                                    {
                                        c.For<IDevice>().Use(device).Transient();
                                        c.For<IDatabaseContext>().Use(database).Transient();
                                    });
        }

        [Export("AddTestVoucher")]
        public void AddTestVoucher(String voucherData)
        {
            if (App.IsIntegrationTestMode == true)
            {
                Voucher voucher = JsonConvert.DeserializeObject<Voucher>(voucherData);
                TestVoucherManagementACLClient voucherManagerAclClient = App.Container.GetInstance<IVoucherManagerACLClient>() as TestVoucherManagementACLClient;
                
                voucherManagerAclClient.Vouchers.Add(voucher);
            }
        }
        
        [Export("GetDeviceIdentifier")]
        public String GetDeviceIdentifier()
        {
            Console.WriteLine("In Get Device Identifier");

            IDevice device = new AndroidDevice();

            return device.GetDeviceIdentifier();
        }

        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="savedInstanceState">State of the saved instance.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;
            FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;

            String connectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TransactionProcessing.db");
            this.Device = new AndroidDevice();
            this.Database = new DatabaseContext(connectionString);

            base.OnCreate(savedInstanceState);

            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += this.TaskSchedulerOnUnobservedTaskException;

            Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            
            this.LoadApplication(new App(this.Device, this.Database));
        }

        

        /// <summary>
        /// Currents the domain on unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="unhandledExceptionEventArgs">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomainOnUnhandledException(Object sender,
                                                       UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Exception newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            
            Task.Run(async () => { await this.Database.InsertLogMessages(DatabaseContext.CreateFatalLogMessages(newExc)); });
        }

        /// <summary>
        /// Tasks the scheduler on unobserved task exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="unobservedTaskExceptionEventArgs">The <see cref="UnobservedTaskExceptionEventArgs"/> instance containing the event data.</param>
        private void TaskSchedulerOnUnobservedTaskException(Object sender,
                                                            UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Exception newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);

            Task.Run(async () => { await this.Database.InsertLogMessages(DatabaseContext.CreateFatalLogMessages(newExc)); });
        }

        #endregion
    }
}