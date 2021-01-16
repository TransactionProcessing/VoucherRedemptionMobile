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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IRedemptionFailedPage" />
    /// <seealso cref="VoucherRedemptionMobile.Pages.IPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RedemptionFailedPage : ContentPage, IRedemptionFailedPage, IPage
    {
        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedemptionFailedPage"/> class.
        /// </summary>
        /// <param name="loggingDatabase">The logging database.</param>
        public RedemptionFailedPage(IDatabaseContext loggingDatabase)
        {
            this.Database = loggingDatabase;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            this.InitializeComponent();

            this.CancelButton.Clicked += this.CancelButton_Clicked;
        }

        /// <summary>
        /// Occurs when [complete button clicked].
        /// </summary>
        public event EventHandler CancelButtonClicked;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} Init"));
        }

        /// <summary>
        /// Handles the Clicked event of the CompleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CancelButton_Clicked(Object sender,
                                          EventArgs e)
        {
            this.CancelButtonClicked(sender, e);
        }
    }
}