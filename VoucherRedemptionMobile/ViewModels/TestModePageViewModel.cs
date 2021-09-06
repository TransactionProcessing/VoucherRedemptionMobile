namespace VoucherRedemptionMobile.ViewModels
{
    using System;
    using Xamarin.Forms;

    public class TestModePageViewModel : BindableObject
    {
        public TestModePageViewModel()
        {
        }

        private string pinNumber;

        private string testVoucherData;

        public String PinNumber
        {
            get
            {
                return this.pinNumber;
            }
            set
            {
                this.pinNumber = value;
                this.OnPropertyChanged(nameof(this.PinNumber));
            }
        }

        public String TestVoucherData
        {
            get
            {
                return this.testVoucherData;
            }
            set
            {
                this.testVoucherData = value;
                this.OnPropertyChanged(nameof(this.TestVoucherData));
            }
        }
    }
}