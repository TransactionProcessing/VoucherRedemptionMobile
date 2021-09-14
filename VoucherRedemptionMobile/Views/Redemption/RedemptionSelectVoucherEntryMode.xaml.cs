using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoucherRedemptionMobile.Views.Redemption
{
    using Database;
    using Pages;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RedemptionSelectVoucherEntryMode :ContentPage, IRedemptionSelectVoucherEntryModePage, IPage
    {
        private readonly IDatabaseContext Database;
        public RedemptionSelectVoucherEntryMode(IDatabaseContext database)
        {
            this.Database = database;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            InitializeComponent();

            this.KeyEntryButton.Clicked += this.KeyEntryButton_Clicked;
            this.ScanButton.Clicked += this.ScanButton_Clicked;
        }

        private void KeyEntryButton_Clicked(Object sender,
                                            EventArgs e)
        {
            this.KeyEntryButtonClick(sender, e);
        }

        private void ScanButton_Clicked(Object sender,
                                        EventArgs e)
        {
            this.ScanButtonClick(sender, e);
        }

        public event EventHandler KeyEntryButtonClick;

        public event EventHandler ScanButtonClick;

        public void Init()
        {
        }
    }
}