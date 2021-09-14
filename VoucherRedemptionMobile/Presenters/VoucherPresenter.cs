namespace VoucherRedemptionMobile.Presenters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Database;
    using Newtonsoft.Json;
    using Pages;
    using Plugin.Toast;
    using ViewModels;
    using VoucherManagementACL.DataTransferObjects.Responses;
    using VoucherRedemption.Clients;
    using Xamarin.Forms;
    using ZXing;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.Presenters.IVoucherPresenter" />
    public class VoucherPresenter : IVoucherPresenter
    {
        #region Fields

        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// The device
        /// </summary>
        private readonly IDevice Device;

        /// <summary>
        /// The redemption enter voucher code page
        /// </summary>
        private readonly IRedemptionEnterVoucherCodePage RedemptionEnterVoucherCodePage;

        /// <summary>
        /// The redemption enter voucher code view model
        /// </summary>
        private readonly RedemptionEnterVoucherCodeViewModel RedemptionEnterVoucherCodeViewModel;

        /// <summary>
        /// The redemption failed page
        /// </summary>
        private readonly IRedemptionFailedPage RedemptionFailedPage;

        /// <summary>
        /// The redemption scan voucher code page
        /// </summary>
        private readonly IRedemptionScanVoucherCodePage RedemptionScanVoucherCodePage;

        /// <summary>
        /// The redemption select entry mode page
        /// </summary>
        private readonly IRedemptionSelectVoucherEntryModePage RedemptionSelectEntryModePage;

        /// <summary>
        /// The redemption success page
        /// </summary>
        private readonly IRedemptionSuccessPage RedemptionSuccessPage;

        /// <summary>
        /// The redemption voucher details page
        /// </summary>
        private readonly IRedemptionVoucherDetailsPage RedemptionVoucherDetailsPage;

        /// <summary>
        /// The redemption voucher details view model
        /// </summary>
        private readonly RedemptionVoucherDetailsViewModel RedemptionVoucherDetailsViewModel;

        /// <summary>
        /// The voucher manager acl client
        /// </summary>
        private readonly IVoucherManagerACLClient VoucherManagerAclClient;

        /// <summary>
        /// The voucher page
        /// </summary>
        private readonly IVoucherPage VoucherPage;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherPresenter" /> class.
        /// </summary>
        /// <param name="voucherPage">The voucher page.</param>
        /// <param name="redemptionSelectEntryModePage">The redemption select entry mode page.</param>
        /// <param name="redemptionEnterVoucherCodePage">The redemption enter voucher code page.</param>
        /// <param name="redemptionScanVoucherCodePage">The redemption scan voucher code page.</param>
        /// <param name="redemptionVoucherDetailsPage">The redemption voucher details page.</param>
        /// <param name="redemptionEnterVoucherCodeViewModel">The redemption enter voucher code view model.</param>
        /// <param name="redemptionVoucherDetailsViewModel">The redemption voucher details view model.</param>
        /// <param name="redemptionSuccessPage">The redemption success page.</param>
        /// <param name="redemptionFailedPage">The redemption failed page.</param>
        /// <param name="voucherManagerAclClient">The voucher manager acl client.</param>
        /// <param name="device">The device.</param>
        /// <param name="database">The database.</param>
        public VoucherPresenter(IVoucherPage voucherPage,
                                IRedemptionSelectVoucherEntryModePage redemptionSelectEntryModePage,
                                IRedemptionEnterVoucherCodePage redemptionEnterVoucherCodePage,
                                IRedemptionScanVoucherCodePage redemptionScanVoucherCodePage,
                                IRedemptionVoucherDetailsPage redemptionVoucherDetailsPage,
                                RedemptionEnterVoucherCodeViewModel redemptionEnterVoucherCodeViewModel,
                                RedemptionVoucherDetailsViewModel redemptionVoucherDetailsViewModel,
                                IRedemptionSuccessPage redemptionSuccessPage,
                                IRedemptionFailedPage redemptionFailedPage,
                                IVoucherManagerACLClient voucherManagerAclClient,
                                IDevice device,
                                IDatabaseContext database)
        {
            this.VoucherPage = voucherPage;
            this.RedemptionSelectEntryModePage = redemptionSelectEntryModePage;
            this.RedemptionEnterVoucherCodePage = redemptionEnterVoucherCodePage;
            this.RedemptionScanVoucherCodePage = redemptionScanVoucherCodePage;
            this.RedemptionVoucherDetailsPage = redemptionVoucherDetailsPage;
            this.RedemptionEnterVoucherCodeViewModel = redemptionEnterVoucherCodeViewModel;
            this.RedemptionVoucherDetailsViewModel = redemptionVoucherDetailsViewModel;
            this.RedemptionSuccessPage = redemptionSuccessPage;
            this.RedemptionFailedPage = redemptionFailedPage;
            this.VoucherManagerAclClient = voucherManagerAclClient;
            this.Device = device;
            this.Database = database;

            this.VoucherPage.VoucherRedemptionButtonClick += this.VoucherPage_VoucherRedemptionButtonClick;

            this.RedemptionSelectEntryModePage.KeyEntryButtonClick += this.RedemptionSelectEntryModePage_KeyEntryButtonClick;
            this.RedemptionSelectEntryModePage.ScanButtonClick += this.RedemptionSelectEntryModePage_ScanButtonClick;

            this.RedemptionEnterVoucherCodePage.FindVoucherButtonClick += this.RedemptionEnterVoucherCodePage_FindVoucherButtonClick;
            this.RedemptionEnterVoucherCodePage.CancelButtonClick += this.RedemptionEnterVoucherCodePage_CancelButtonClick;

            this.RedemptionScanVoucherCodePage.VoucherBarcodeScanned += this.RedemptionScanVoucherCodePage_VoucherBarcodeScanned;
            this.RedemptionScanVoucherCodePage.CancelButtonClick += this.RedemptionScanVoucherCodePage_CancelButtonClick;

            this.RedemptionVoucherDetailsPage.CancelButtonClick += this.RedemptionVoucherDetailsPage_CancelButtonClick;
            this.RedemptionVoucherDetailsPage.RedeemVoucherButtonClick += this.RedemptionVoucherDetailsPage_RedeemVoucherButtonClick;

            this.RedemptionSuccessPage.CompleteButtonClicked += this.RedemptionSuccessPage_CompleteButtonClicked;
            this.RedemptionFailedPage.CancelButtonClicked += this.RedemptionFailedPage_CancelButtonClicked;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public async Task Start()
        {
            this.VoucherPage.Init();
            await Application.Current.MainPage.Navigation.PushAsync((Page)this.VoucherPage);
        }

        /// <summary>
        /// Displays the voucher details.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        private async Task DisplayVoucherDetails(String voucherCode)
        {
            GetVoucherResponseMessage voucherDetails = await this.GetVoucherDetails(voucherCode, CancellationToken.None);

            if (voucherDetails == null)
            {
                CrossToastPopUp.Current.ShowToastWarning($"No voucher details found with code {voucherCode}");
                return;
            }

            // TODO: Create a factory
            this.RedemptionVoucherDetailsViewModel.ExpiryDate = voucherDetails.ExpiryDate;
            this.RedemptionVoucherDetailsViewModel.Value = voucherDetails.Value;
            this.RedemptionVoucherDetailsViewModel.VoucherCode = voucherDetails.VoucherCode;

            this.RedemptionVoucherDetailsPage.Init(this.RedemptionVoucherDetailsViewModel);

            await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionVoucherDetailsPage);
        }

        /// <summary>
        /// Gets the voucher details.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<GetVoucherResponseMessage> GetVoucherDetails(String voucherCode,
                                                                        CancellationToken cancellationToken)
        {
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"About to get voucher details for voucher code {voucherCode}"));

            GetVoucherResponseMessage voucherDetails = null;

            await
                this.Database.InsertLogMessage(DatabaseContext
                                                   .CreateInformationLogMessage($"Message Sent to Host Application version [{this.Device.GetSoftwareVersion()}] Voucher code [{voucherCode}]"));

            voucherDetails = await this.VoucherManagerAclClient.GetVoucher(App.TokenResponse.AccessToken,
                                                                           this.Device.GetSoftwareVersion(),
                                                                           voucherCode,
                                                                           cancellationToken);

            String responseJson = JsonConvert.SerializeObject(voucherDetails);
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"Message Rcv from Host [{responseJson}]"));

            if (voucherDetails.ResponseCode != "0000")
            {
                return null;
            }

            return voucherDetails;
        }

        /// <summary>
        /// Redeems the voucher.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<RedeemVoucherResponseMessage> RedeemVoucher(String voucherCode,
                                                                       CancellationToken cancellationToken)
        {
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"About to redeem voucher for voucher code {voucherCode}"));

            RedeemVoucherResponseMessage voucherRedemptionResponse = null;

            await
                this.Database.InsertLogMessage(DatabaseContext
                                                   .CreateInformationLogMessage($"Message Sent to Host Application version [{this.Device.GetSoftwareVersion()}] Voucher code [{voucherCode}]"));

            voucherRedemptionResponse =
                await this.VoucherManagerAclClient.RedeemVoucher(App.TokenResponse.AccessToken, this.Device.GetSoftwareVersion(), voucherCode, cancellationToken);

            String responseJson = JsonConvert.SerializeObject(voucherRedemptionResponse);
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"Message Rcv from Host [{responseJson}]"));

            if (voucherRedemptionResponse.ResponseCode != "0000")
            {
                return null;
            }

            return voucherRedemptionResponse;
        }

        /// <summary>
        /// Handles the CancelButtonClick event of the RedemptionEnterVoucherCodePage control.
        /// </summary>
        /// <param name="sernder">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void RedemptionEnterVoucherCodePage_CancelButtonClick(Object sernder,
                                                                            EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        /// <summary>
        /// Handles the FindVoucherButtonClick event of the RedemptionEnterVoucherCodePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void RedemptionEnterVoucherCodePage_FindVoucherButtonClick(Object sender,
                                                                                 EventArgs e)
        {
            if (string.IsNullOrEmpty(this.RedemptionEnterVoucherCodeViewModel.VoucherCode))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Voucher Code", "Please enter a valid voucher code to continue", "OK");
            }
            else
            {
                await this.DisplayVoucherDetails(this.RedemptionEnterVoucherCodeViewModel.VoucherCode);
            }
        }

        /// <summary>
        /// Handles the CancelButtonClicked event of the RedemptionFailedPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void RedemptionFailedPage_CancelButtonClicked(Object sender,
                                                                    EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Handles the CancelButtonClick event of the RedemptionScanVoucherCodePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionScanVoucherCodePage_CancelButtonClick(Object sender,
                                                                           EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        /// <summary>
        /// Redemptions the scan voucher code page voucher barcode scanned.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void RedemptionScanVoucherCodePage_VoucherBarcodeScanned(Object sender,
                                                                               Result e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                                         {
                                                             Helpers.Vibrate(1);

                                                             String voucherCode = e.Text;
                                                             if (string.IsNullOrEmpty(voucherCode))
                                                             {
                                                                 await Application.Current.MainPage.DisplayAlert("Invalid Voucher Code",
                                                                                                                     "Please scan a valid voucher code to continue",
                                                                                                                     "OK");
                                                             }
                                                             else
                                                             {
                                                                 await this.DisplayVoucherDetails(voucherCode);
                                                             }
                                                         });
        }

        /// <summary>
        /// Handles the KeyEntryButtonClick event of the RedemptionSelectEntryModePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionSelectEntryModePage_KeyEntryButtonClick(Object sender,
                                                                             EventArgs e)
        {
            this.RedemptionEnterVoucherCodePage.Init(this.RedemptionEnterVoucherCodeViewModel);

            await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionEnterVoucherCodePage);
        }

        /// <summary>
        /// Handles the ScanButtonClick event of the RedemptionSelectEntryModePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionSelectEntryModePage_ScanButtonClick(Object sender,
                                                                         EventArgs e)
        {
            this.RedemptionScanVoucherCodePage.Init();

            await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionScanVoucherCodePage);
        }

        /// <summary>
        /// Handles the CompleteButtonClicked event of the RedemptionSuccessPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void RedemptionSuccessPage_CompleteButtonClicked(Object sender,
                                                                       EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Handles the CancelButtonClick event of the RedemptionVoucherDetailsPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void RedemptionVoucherDetailsPage_CancelButtonClick(Object sender,
                                                                          EventArgs e)
        {
            // Not sure if we have scanned or keyed :|
            this.RedemptionEnterVoucherCodePage.ClearVoucherCode();
            this.RedemptionScanVoucherCodePage.Init();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        /// <summary>
        /// Handles the RedeemVoucherButtonClick event of the RedemptionVoucherDetailsPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void RedemptionVoucherDetailsPage_RedeemVoucherButtonClick(Object sender,
                                                                                 EventArgs e)
        {
            RedeemVoucherResponseMessage redemptionResponse = await this.RedeemVoucher(this.RedemptionVoucherDetailsViewModel.VoucherCode, CancellationToken.None);

            // TODO: View model with balance
            if (redemptionResponse == null)
            {
                await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionFailedPage);
            }
            else
            {
                await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionSuccessPage);
            }
        }

        /// <summary>
        /// Handles the VoucherRedemptionButtonClick event of the VoucherPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void VoucherPage_VoucherRedemptionButtonClick(Object sender,
                                                                    EventArgs e)
        {
            this.RedemptionSelectEntryModePage.Init();
            await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionSelectEntryModePage);
        }

        #endregion
    }
}