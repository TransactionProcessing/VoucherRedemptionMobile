namespace VoucherRedemption.Clients
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientProxyBase;
    using Newtonsoft.Json;
    using VoucherManagementACL.DataTransferObjects.Responses;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ClientProxyBase" />
    /// <seealso cref="IVoucherManagerACLClient" />
    public class VoucherManagerACLClient : ClientProxyBase, IVoucherManagerACLClient
    {
        #region Fields

        /// <summary>
        /// The base address resolver
        /// </summary>
        private readonly Func<String, String> BaseAddressResolver;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherManagerACLClient"/> class.
        /// </summary>
        /// <param name="baseAddressResolver">The base address resolver.</param>
        /// <param name="httpClient">The HTTP client.</param>
        public VoucherManagerACLClient(Func<String, String> baseAddressResolver,
                                       HttpClient httpClient) : base(httpClient)
        {
            this.BaseAddressResolver = baseAddressResolver;

            // Add the API version header
            this.HttpClient.DefaultRequestHeaders.Add("api-version", "1.0");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the voucher.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<GetVoucherResponseMessage> GetVoucher(String accessToken,
                                                                String applicationVersion,
                                                                String voucherCode,
                                                                CancellationToken cancellationToken)
        {
            GetVoucherResponseMessage response = null;
            String requestUri = this.BuildRequestUrl($"/api/vouchers?applicationVersion={applicationVersion}&voucherCode={voucherCode}");

            try
            {
                // Add the access token to the client headers
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.GetAsync(requestUri, cancellationToken);

                // Process the response
                String content = await this.HandleResponse(httpResponse, cancellationToken);

                // call was successful so now deserialise the body to the response object
                response = JsonConvert.DeserializeObject<GetVoucherResponseMessage>(content);
            }
            catch(Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception($"Error getting voucher for voucher code {voucherCode}.", ex);

                throw exception;
            }

            return response;
        }

        /// <summary>
        /// Redeems the voucher.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<RedeemVoucherResponseMessage> RedeemVoucher(String accessToken,
                                                                      String applicationVersion,
                                                                      String voucherCode,
                                                                      CancellationToken cancellationToken)
        {
            RedeemVoucherResponseMessage response = null;
            String requestUri = this.BuildRequestUrl($"/api/vouchers?applicationVersion={applicationVersion}&voucherCode={voucherCode}");

            try
            {
                // Add the access token to the client headers
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.PutAsync(requestUri, new StringContent(string.Empty), cancellationToken);

                // Process the response
                String content = await this.HandleResponse(httpResponse, cancellationToken);

                // call was successful so now deserialise the body to the response object
                response = JsonConvert.DeserializeObject<RedeemVoucherResponseMessage>(content);
            }
            catch(Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception($"Error redeeming voucher with voucher code {voucherCode}.", ex);

                throw exception;
            }

            return response;
        }

        /// <summary>
        /// Builds the request URL.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        private String BuildRequestUrl(String route)
        {
            String baseAddress = this.BaseAddressResolver("VoucherManagementACL");

            String requestUri = $"{baseAddress}{route}";

            return requestUri;
        }

        #endregion
    }
}