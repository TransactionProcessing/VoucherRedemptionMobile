namespace VoucherRedemption.Clients
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using VoucherManagementACL.DataTransferObjects.Responses;

    /// <summary>
    /// 
    /// </summary>
    public interface IVoucherManagerACLClient
    {
        #region Methods

        /// <summary>
        /// Gets the voucher.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<GetVoucherResponseMessage> GetVoucher(String accessToken,
                                                   String applicationVersion,
                                            String voucherCode,
                                            CancellationToken cancellationToken);

        /// <summary>
        /// Redeems the voucher.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<RedeemVoucherResponseMessage> RedeemVoucher(String accessToken,
                                                         String applicationVersion,
                                                         String voucherCode,
                                                         CancellationToken cancellationToken);

        #endregion
    }
}