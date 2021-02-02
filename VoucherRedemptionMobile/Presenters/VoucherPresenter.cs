namespace VoucherRedemptionMobile.Presenters
{
    using System;
    using System.Collections.Generic;
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

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="VoucherRedemptionMobile.Presenters.IVoucherPresenter" />
    public class VoucherPresenter : IVoucherPresenter
    {
        /// <summary>
        /// The voucher page
        /// </summary>
        private readonly IVoucherPage VoucherPage;

        /// <summary>
        /// The redemption enter voucher code page
        /// </summary>
        private readonly IRedemptionEnterVoucherCodePage RedemptionEnterVoucherCodePage;

        /// <summary>
        /// The redemption voucher details page
        /// </summary>
        private readonly IRedemptionVoucherDetailsPage RedemptionVoucherDetailsPage;

        /// <summary>
        /// The redemption enter voucher code view model
        /// </summary>
        private readonly RedemptionEnterVoucherCodeViewModel RedemptionEnterVoucherCodeViewModel;

        /// <summary>
        /// The redemption voucher details view model
        /// </summary>
        private readonly RedemptionVoucherDetailsViewModel RedemptionVoucherDetailsViewModel;

        /// <summary>
        /// The redemption success page
        /// </summary>
        private readonly IRedemptionSuccessPage RedemptionSuccessPage;

        /// <summary>
        /// The redemption failed page
        /// </summary>
        private readonly IRedemptionFailedPage RedemptionFailedPage;

        /// <summary>
        /// The voucher manager acl client
        /// </summary>
        private readonly IVoucherManagerACLClient VoucherManagerAclClient;

        /// <summary>
        /// The device
        /// </summary>
        private readonly IDevice Device;

        /// <summary>
        /// The database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherPresenter" /> class.
        /// </summary>
        /// <param name="voucherPage">The voucher page.</param>
        /// <param name="redemptionEnterVoucherCodePage">The redemption enter voucher code page.</param>
        /// <param name="redemptionVoucherDetailsPage">The redemption voucher details page.</param>
        /// <param name="redemptionEnterVoucherCodeViewModel">The redemption enter voucher code view model.</param>
        /// <param name="redemptionVoucherDetailsViewModel">The redemption voucher details view model.</param>
        /// <param name="redemptionSuccessPage">The redemption success page.</param>
        /// <param name="redemptionFailedPage">The redemption failed page.</param>
        /// <param name="voucherManagerAclClient">The voucher manager acl client.</param>
        /// <param name="device">The device.</param>
        /// <param name="database">The database.</param>
        public VoucherPresenter(IVoucherPage voucherPage,
                                IRedemptionEnterVoucherCodePage redemptionEnterVoucherCodePage,
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
            this.RedemptionEnterVoucherCodePage = redemptionEnterVoucherCodePage;
            this.RedemptionVoucherDetailsPage = redemptionVoucherDetailsPage;
            this.RedemptionEnterVoucherCodeViewModel = redemptionEnterVoucherCodeViewModel;
            this.RedemptionVoucherDetailsViewModel = redemptionVoucherDetailsViewModel;
            this.RedemptionSuccessPage = redemptionSuccessPage;
            this.RedemptionFailedPage = redemptionFailedPage;
            this.VoucherManagerAclClient = voucherManagerAclClient;
            this.Device = device;
            this.Database = database;

            this.VoucherPage.VoucherRedemptionButtonClick += this.VoucherPage_VoucherRedemptionButtonClick;

            this.RedemptionEnterVoucherCodePage.FindVoucherButtonClick += this.RedemptionEnterVoucherCodePage_FindVoucherButtonClick;
            this.RedemptionEnterVoucherCodePage.CancelButtonClick += this.RedemptionEnterVoucherCodePage_CancelButtonClick;

            this.RedemptionVoucherDetailsPage.CancelButtonClick += this.RedemptionVoucherDetailsPage_CancelButtonClick;
            this.RedemptionVoucherDetailsPage.RedeemVoucherButtonClick += this.RedemptionVoucherDetailsPage_RedeemVoucherButtonClick;

            this.RedemptionSuccessPage.CompleteButtonClicked += this.RedemptionSuccessPage_CompleteButtonClicked;
            this.RedemptionFailedPage.CancelButtonClicked += this.RedemptionFailedPage_CancelButtonClicked;
        }

        /// <summary>
        /// Handles the CancelButtonClicked event of the RedemptionFailedPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionFailedPage_CancelButtonClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Handles the CompleteButtonClicked event of the RedemptionSuccessPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionSuccessPage_CompleteButtonClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public async Task Start()
        {
            this.VoucherPage.Init();
            await Application.Current.MainPage.Navigation.PushAsync((Page)this.VoucherPage);
        }

        /// <summary>
        /// Handles the VoucherRedemptionButtonClick event of the VoucherPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void VoucherPage_VoucherRedemptionButtonClick(object sender, System.EventArgs e)
        {
            this.RedemptionEnterVoucherCodePage.Init(this.RedemptionEnterVoucherCodeViewModel);

            await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionEnterVoucherCodePage);
        }

        /// <summary>
        /// Handles the FindVoucherButtonClick event of the RedemptionEnterVoucherCodePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void RedemptionEnterVoucherCodePage_FindVoucherButtonClick(object sender, System.EventArgs e)
        {
            if (String.IsNullOrEmpty(this.RedemptionEnterVoucherCodeViewModel.VoucherCode))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Voucher Code", "Please enter a valid voucher code to continue", "OK");
            }
            else
            {
                GetVoucherResponseMessage voucherDetails = await this.GetVoucherDetails(this.RedemptionEnterVoucherCodeViewModel.VoucherCode, CancellationToken.None);

                if (voucherDetails == null)
                {
                    CrossToastPopUp.Current.ShowToastWarning($"No voucher details found with code {this.RedemptionEnterVoucherCodeViewModel.VoucherCode}");
                    return;
                }

                // TODO: Create a factory
                this.RedemptionVoucherDetailsViewModel.ExpiryDate = voucherDetails.ExpiryDate;
                this.RedemptionVoucherDetailsViewModel.Value = voucherDetails.Value;
                this.RedemptionVoucherDetailsViewModel.VoucherCode = voucherDetails.VoucherCode;

                this.RedemptionVoucherDetailsPage.Init(this.RedemptionVoucherDetailsViewModel);

                await Application.Current.MainPage.Navigation.PushAsync((Page)this.RedemptionVoucherDetailsPage);
            }
        }

        /// <summary>
        /// Gets the voucher details.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task<GetVoucherResponseMessage> GetVoucherDetails(String voucherCode, CancellationToken cancellationToken)
        {
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"About to get voucher details for voucher code {voucherCode}"));

            GetVoucherResponseMessage voucherDetails = null;
            
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"Message Sent to Host Application version [{this.Device.GetSoftwareVersion()}] Voucher code [{voucherCode}]"));

            voucherDetails =
                await this.VoucherManagerAclClient.GetVoucher(App.TokenResponse.AccessToken,
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
        private async Task<RedeemVoucherResponseMessage> RedeemVoucher(String voucherCode, CancellationToken cancellationToken)
        {
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"About to redeem voucher for voucher code {voucherCode}"));

            RedeemVoucherResponseMessage voucherRedemptionResponse = null;
            
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"Message Sent to Host Application version [{this.Device.GetSoftwareVersion()}] Voucher code [{voucherCode}]"));

            voucherRedemptionResponse =
                await this.VoucherManagerAclClient.RedeemVoucher(App.TokenResponse.AccessToken,
                                                                 this.Device.GetSoftwareVersion(),
                                                              voucherCode,
                                                              cancellationToken);

            String responseJson = JsonConvert.SerializeObject(voucherRedemptionResponse);
            await this.Database.InsertLogMessage(DatabaseContext.CreateInformationLogMessage($"Message Rcv from Host [{responseJson}]"));

            if (voucherRedemptionResponse.ResponseCode != "0000")
            {
                return null;
            }

            return voucherRedemptionResponse;
        }

        /// <summary>
        /// Handles the RedeemVoucherButtonClick event of the RedemptionVoucherDetailsPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void RedemptionVoucherDetailsPage_RedeemVoucherButtonClick(object sender, System.EventArgs e)
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
        /// Handles the CancelButtonClick event of the RedemptionEnterVoucherCodePage control.
        /// </summary>
        /// <param name="sernder">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void RedemptionEnterVoucherCodePage_CancelButtonClick(object sernder,
                                                                            EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        /// <summary>
        /// Handles the CancelButtonClick event of the RedemptionVoucherDetailsPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void RedemptionVoucherDetailsPage_CancelButtonClick(object sender, System.EventArgs e)
        {
            this.RedemptionEnterVoucherCodePage.ClearVoucherCode();
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}