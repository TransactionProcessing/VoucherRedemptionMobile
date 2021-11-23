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
            //this.ViewModel.TestUserData = "i65W8ixJzTVUslIqSk1JzS0oyczPKy1OLXIoSS0uQQgZ6iXn65VmK+mAlRsBlRsaGZuYminVxgIA";
            //this.ViewModel.TestVoucherData =
            //    "zZVNaxsxEIb/y17rMRpp9OVTaRpKDrkEk0vpYSSNWlPHDrtraCn575WdxjZJycG0JsvCrsbz6lmsB+nzr+4D93ldpJt13aS7HEYe5aq0kSGfQy4EjJGAEC2wcRXYW7aiKVNMLXGxXo0953GXqdmIrcaBJWagxBpCqgGsichZKBXaZi5/3C/6nx8bqWW00gioQZs52pnBmfZTrR05/06pmVKt/2r4JCvpW3+DjP1GtqWrYdgcj2+kiNxtK5WXw660bdhS5ou7AwkPJKN3JI970p7zWuzPBx7FnthHKaUUwu6e77oeG69lGPjrnz/7RvLifiGr8fKOF8tWG2UY+6civt+/TvN6uvl+nLhep8WyTbPaLJeTbt7zamhrsFivdsuAYslggyfRDih7gVSRwEVlvZdQnLg22y0vN20OVFOlJs2DJa/yYXy73uRv0l88uqGeLuz2P+1YmorSQVlA4saSWiCg9eCIU0QVQ0XdPUzerGfemzN55j2d4tlx7H94pk/3TAdrKJoAFdkCsTAEGyuQt1HnZG3CfPBMP/NMv+qZfuaZD5RD0hlMVgnIKA+hNM8CFsfZBm4avWXPIoUzeRbtKZpFiv9Ys0dnXqjUoTZknQ+xe2lUFmYpqoJqD6CAHjgGAV8zOpcl6qRP3bnMM6MyGW22rFCk2UuOmwvGQpOiFEKfioQ3bJRXNp7HKK/cCUYdp84jVPiLUNUFZaRGsJzbGqmMwDU58M7Z7DKHEvnULYqeCZVCbadutNCOX91YbZ+KkTzkbIzT0aZ2VHYPX34D";
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