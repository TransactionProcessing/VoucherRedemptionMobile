using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VoucherRedemptionMobile.Views
{
    using System.Diagnostics.CodeAnalysis;
    using Database;
    using Pages;
    using ViewModels;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    [ExcludeFromCodeCoverage]
    public partial class TestModePage : ContentPage, ITestModePage, IPage
    {
        #region Fields


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        /// <param name="database">The logging database.</param>
        public TestModePage(IDatabaseContext database)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [profile button clicked].
        /// </summary>
        public event EventHandler SetTestModeButtonClick;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="viewModel"></param>
        public void Init(TestModePageViewModel viewModel)
        {
            this.SetTestModeButton.Clicked += SetTestModeButton_Clicked;
            this.ViewModel = viewModel;
            this.BindingContext = this.ViewModel;
        }

        private void SetTestModeButton_Clicked(object sender, EventArgs e)
        {
            this.SetTestModeButtonClick(sender, e);
        }

        public TestModePageViewModel ViewModel { get; set; }





        #endregion
    }
}