namespace VoucherRedemptionMobile.Common
{
    using System;

    public class TestDevice : IDevice
    {
        public String GetDeviceIdentifier()
        {
            return String.Empty;
        }

        public String GetSoftwareVersion()
        {
            return String.Empty;
        }
    }
}