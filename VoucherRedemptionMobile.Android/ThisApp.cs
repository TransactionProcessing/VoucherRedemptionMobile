namespace VoucherRedemptionMobile.Droid
{
    using System;
    using Android.App;
    using Android.Runtime;
    using Common;
    using Database;
    using Java.Interop;
    using Java.Lang;
    using Newtonsoft.Json;
    using TestClients;
    using TestClients.Models;
    using Unity;
    using Unity.Lifetime;
    using VoucherRedemption.Clients;
    using String = Java.Lang.String;

    [Preserve(AllMembers = true)]
    [Application]
    public class ThisApp : Application
    {
        protected ThisApp(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        [Export("SetIntegrationTestModeOn")]
        public void SetIntegrationTestModeOn(Java.Lang.String arg)
        {
            Console.WriteLine($"Inside SetIntegrationTestModeOn");
            App.IsIntegrationTestMode = true;

            App.Container = Bootstrapper.Run();

            IDevice device = new AndroidDevice();
            IDatabaseContext database = new DatabaseContext(System.String.Empty);
            App.Container.RegisterInstance(typeof(IDatabaseContext), database, new ContainerControlledLifetimeManager());
            App.Container.RegisterInstance(typeof(IDevice), device, new ContainerControlledLifetimeManager());
        }

        [Export("AddTestVoucher")]
        public void AddTestVoucher(System.String voucherData)
        {
            if (App.IsIntegrationTestMode == true)
            {
                Voucher voucher = JsonConvert.DeserializeObject<Voucher>(voucherData.ToString());
                TestVoucherManagementACLClient voucherManagerAclClient = App.Container.Resolve<IVoucherManagerACLClient>() as TestVoucherManagementACLClient;

                voucherManagerAclClient.Vouchers.Add(voucher);
            }
        }


    }


}