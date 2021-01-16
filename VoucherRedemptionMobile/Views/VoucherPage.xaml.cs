using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoucherRedemptionMobile.Views
{
    using Common;
    using Database;
    using Pages;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IVoucherPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VoucherPage : ContentPage, IVoucherPage, IPage
    {
        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherPage"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="device">The device.</param>
        public VoucherPage(IDatabaseContext database,
                           IDevice device)
        {
            this.Database = database;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        public void Init()
        {
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} Init"));

            this.VoucherRedemptionButton.Clicked += this.VoucherRedemptionButton_Clicked;
        }

        /// <summary>
        /// Handles the Clicked event of the VoucherRedemptionButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VoucherRedemptionButton_Clicked(object sender, EventArgs e)
        {
            this.VoucherRedemptionButtonClick(sender, e);
        }

        /// <summary>
        /// Occurs when [voucher redemption button click].
        /// </summary>
        public event EventHandler VoucherRedemptionButtonClick;
    }
}