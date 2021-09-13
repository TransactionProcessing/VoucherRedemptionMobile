namespace VoucherRedemptionMobile.Droid
{
    using System;
    using Android.App;
    using Android.Runtime;
    using Common;
    using Database;
    using Java.Interop;
    using Newtonsoft.Json;
    using TestClients;
    using TestClients.Models;
    using Unity;
    using Unity.Lifetime;
    using VoucherRedemption.Clients;

    [Preserve(AllMembers = true)]
    [Application]
    public class ThisApp : Application
    {
        protected ThisApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        //[Export("SetIntegrationTestModeOn")]
        //public void SetIntegrationTestModeOn(String arg)
        //{
        //    Console.WriteLine($"Inside SetIntegrationTestModeOn");
        //    App.IsIntegrationTestMode = true;
        //    App.Container = Bootstrapper.Run();

        //    IDevice device = new AndroidDevice();
        //    IDatabaseContext database = new DatabaseContext(String.Empty);
        //    App.Container.RegisterInstance(typeof(IDatabaseContext), database, new ContainerControlledLifetimeManager());
        //    App.Container.RegisterInstance(typeof(IDevice), device, new ContainerControlledLifetimeManager());
        //}
        [Export("SetIntegrationTestModeOn")]
        public void SetIntegrationTestModeOn(String arg)
        {
            Console.WriteLine($"Inside SetIntegrationTestModeOn");
            App.IsIntegrationTestMode = true;

            App.Container = Bootstrapper.Run();

            IDevice device = new AndroidDevice();
            IDatabaseContext database = new DatabaseContext(String.Empty);
            App.Container.RegisterInstance(typeof(IDatabaseContext), database, new ContainerControlledLifetimeManager());
            App.Container.RegisterInstance(typeof(IDevice), device, new ContainerControlledLifetimeManager());
        }

        [Export("AddTestVoucher")]
        public void AddTestVoucher(String voucherData)
        {
            if (App.IsIntegrationTestMode == true)
            {
                Voucher voucher = JsonConvert.DeserializeObject<Voucher>(voucherData);
                TestVoucherManagementACLClient voucherManagerAclClient = App.Container.Resolve<IVoucherManagerACLClient>() as TestVoucherManagementACLClient;

                voucherManagerAclClient.Vouchers.Add(voucher);
            }
        }


    }
}