namespace VoucherRedemptionMobile.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Database;
    using Database.Entities;
    using Pages;
    using Services;
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ISupportPresenter" />
    public class SupportPresenter : ISupportPresenter
    {
        #region Fields

        /// <summary>
        /// The configuration service client
        /// </summary>
        private readonly IConfigurationServiceClient ConfigurationServiceClient;

        /// <summary>
        /// The device
        /// </summary>
        private readonly IDevice Device;

        /// <summary>
        /// The logging database
        /// </summary>
        private readonly IDatabaseContext Database;

        /// <summary>
        /// The support page
        /// </summary>
        private readonly ISupportPage SupportPage;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportPresenter"/> class.
        /// </summary>
        /// <param name="supportPage">The support page.</param>
        /// <param name="database">The logging database.</param>
        /// <param name="device">The device.</param>
        /// <param name="configurationServiceClient">The configuration service client.</param>
        public SupportPresenter(ISupportPage supportPage,
                                IDatabaseContext database,
                                IDevice device,
                                IConfigurationServiceClient configurationServiceClient)
        {
            this.SupportPage = supportPage;
            this.Database = database;
            this.Device = device;
            this.ConfigurationServiceClient = configurationServiceClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public async Task Start()
        {
            this.SupportPage.UploadLogsButtonClick += this.SupportPage_UploadLogsButtonClick;

            this.SupportPage.Init();
            await Application.Current.MainPage.Navigation.PushAsync((Page)this.SupportPage);
        }

        /// <summary>
        /// Handles the UploadLogsButtonClick event of the SupportPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void SupportPage_UploadLogsButtonClick(Object sender,
                                                             EventArgs e)
        {
            while (true)
            {
                List<LogMessage> logEntries = await this.Database.GetLogMessages(10);

                if (logEntries.Any() == false)
                {
                    break;
                }

                await this.ConfigurationServiceClient.PostDiagnosticLogs(this.Device.GetDeviceIdentifier(), logEntries, CancellationToken.None);

                // Clear the logs that have been uploaded
                await this.Database.RemoveUploadedMessages(logEntries);
            }
        }

        #endregion
    }
}