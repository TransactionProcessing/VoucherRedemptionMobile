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
    using ViewModels;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IRedemptionVoucherDetailsPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RedemptionVoucherDetailsPage : ContentPage, IRedemptionVoucherDetailsPage, IPage
    {

        /// <summary>
        /// The view model
        /// </summary>
        private RedemptionVoucherDetailsViewModel ViewModel;
        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;
        /// <summary>
        /// Initializes a new instance of the <see cref="RedemptionVoucherDetailsPage"/> class.
        /// </summary>
        /// <param name="loggingDatabase">The logging database.</param>
        public RedemptionVoucherDetailsPage(IDatabaseContext loggingDatabase)
        {
            this.Database = loggingDatabase;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            this.InitializeComponent();

            this.CancelButton.Clicked += this.CancelButton_Clicked;
            this.RedeemVoucherButton.Clicked += RedeemVoucherButton_Clicked;
        }

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void Init(RedemptionVoucherDetailsViewModel viewModel)
        {
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} Init"));

            this.ViewModel = viewModel;
            this.BindingContext = this.ViewModel;
        }

        /// <summary>
        /// Handles the Clicked event of the RedeemVoucherButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RedeemVoucherButton_Clicked(object sender, EventArgs e)
        {
            this.RedeemVoucherButtonClick(sender, e);
        }

        /// <summary>
        /// Handles the Clicked event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            this.CancelButtonClick(sender, e);
        }

        /// <summary>
        /// Occurs when [redeem voucher button click].
        /// </summary>
        public event EventHandler RedeemVoucherButtonClick;

        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        public event EventHandler CancelButtonClick;
    }
}