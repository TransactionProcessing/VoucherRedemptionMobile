namespace VoucherRedemptionMobile.ViewModels
{
    using System;
    using Xamarin.Forms;

    public class MainPageViewModel : BindableObject
    {
        public MainPageViewModel()
        {
            this.availableBalance = "0 KES";
        }

        private String availableBalance;
        public String AvailableBalance
        {
            get
            {
                return this.availableBalance;
            }
            set
            {
                this.availableBalance = value;
                this.OnPropertyChanged(nameof(this.AvailableBalance));
            }
        }
    }
}
