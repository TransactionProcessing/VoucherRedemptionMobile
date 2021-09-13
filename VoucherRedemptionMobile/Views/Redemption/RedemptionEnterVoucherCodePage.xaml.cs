using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoucherRedemptionMobile.Views.Redemption
{
    using Controls;
    using Database;
    using Pages;
    using ViewModels;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IRedemptionEnterVoucherCodePage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RedemptionEnterVoucherCodePage : ContentPage, IRedemptionEnterVoucherCodePage, IPage
    {
        /// <summary>
        /// The view model
        /// </summary>
        private RedemptionEnterVoucherCodeViewModel ViewModel;
        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedemptionEnterVoucherCodePage"/> class.
        /// </summary>
        /// <param name="loggingDatabase">The logging database.</param>
        public RedemptionEnterVoucherCodePage(IDatabaseContext loggingDatabase)
        {
            this.Database = loggingDatabase;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            InitializeComponent();
            
            this.FindVoucherButton.Clicked += this.FindVoucherButton_Clicked;
            this.CancelButton.Clicked += this.CancelButton_Clicked;

            this.VoucherCodeEntry.TextChanged += this.VoucherCodeEntry_TextChanged;
            this.VoucherCodeEntry.Completed += this.VoucherCodeEntry_Completed;
        }

        /// <summary>
        /// Initializes the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void Init(RedemptionEnterVoucherCodeViewModel viewModel)
        {
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} Init"));

            this.ViewModel = viewModel;
            this.BindingContext = this.ViewModel;
        }

        /// <summary>
        /// Clears the voucher code.
        /// </summary>
        public void ClearVoucherCode()
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.VoucherCode = String.Empty;
            }
        }

        /// <summary>
        /// Occurs when [find voucher button click].
        /// </summary>
        public event EventHandler FindVoucherButtonClick;
        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        public event EventHandler CancelButtonClick;

        /// <summary>
        /// Handles the Completed event of the VoucherCodeEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VoucherCodeEntry_Completed(Object sender,
                                                EventArgs e)
        {
            //this.Fin.Focus();
        }

        /// <summary>
        /// Handles the TextChanged event of the CustomerMobileNumberEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void VoucherCodeEntry_TextChanged(Object sender,
                                                           TextChangedEventArgs e)
        {
            //Make sure all characters are numbers
            Boolean isValid = e.NewTextValue.ToCharArray().All(Char.IsDigit);
            if (isValid && String.IsNullOrEmpty(e.NewTextValue) == false)
            {
                this.ViewModel.VoucherCode = e.NewTextValue;
            }
        }

        /// <summary>
        /// Handles the Clicked event of the PerformTopupButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void FindVoucherButton_Clicked(Object sender,
                                                EventArgs e)
        {
            this.FindVoucherButtonClick(sender, e);
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
    }
}