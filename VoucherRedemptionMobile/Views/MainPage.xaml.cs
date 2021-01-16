namespace VoucherRedemptionMobile.Views
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Database;
    using Pages;
    using ViewModels;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    /// <seealso cref="IMainPage" />
    /// <seealso cref="IPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ExcludeFromCodeCoverage]
    public partial class MainPage : ContentPage, IMainPage, IPage
    {
        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        #region Fields


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        /// <param name="database">The logging database.</param>
        public MainPage(IDatabaseContext database)
        {
            this.Database = database;
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} ctor"));
            this.InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [profile button clicked].
        /// </summary>
        public event EventHandler ProfileButtonClicked;

        /// <summary>
        /// Occurs when [reports button clicked].
        /// </summary>
        public event EventHandler ReportsButtonClicked;

        /// <summary>
        /// Occurs when [support button clicked].
        /// </summary>
        public event EventHandler SupportButtonClicked;

        /// <summary>
        /// Occurs when [voucher button clicked].
        /// </summary>
        public event EventHandler VoucherButtonClicked;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="viewModel"></param>
        public void Init(MainPageViewModel viewModel)
        {
            this.Database.InsertLogMessage(DatabaseContext.CreateDebugLogMessage($"In {this.GetType().Name} Init"));
            this.VouchersButton.Clicked += this.VouchersButton_Clicked;
            this.ReportsButton.Clicked += this.ReportsButton_Clicked;
            this.ProfileButton.Clicked += this.ProfileButton_Clicked;
            this.SupportButton.Clicked += this.SupportButton_Clicked;
            this.ViewModel = viewModel;
            this.BindingContext = this.ViewModel;
        }

        private void VouchersButton_Clicked(object sender, EventArgs e)
        {
            this.VoucherButtonClicked(sender, e);
        }

        public MainPageViewModel ViewModel { get; set; }
        
        

        /// <summary>
        /// Handles the Clicked event of the ProfileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ProfileButton_Clicked(Object sender,
                                           EventArgs e)
        {
            this.ProfileButtonClicked(sender, e);
        }

        /// <summary>
        /// Handles the Clicked event of the ReportsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ReportsButton_Clicked(Object sender,
                                           EventArgs e)
        {
            this.ReportsButtonClicked(sender, e);
        }

        /// <summary>
        /// Handles the Clicked event of the SupportButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void SupportButton_Clicked(Object sender,
                                           EventArgs e)
        {
            this.SupportButtonClicked(sender, e);
        }

        #endregion
    }
}