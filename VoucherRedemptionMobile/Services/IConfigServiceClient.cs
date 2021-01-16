namespace VoucherRedemptionMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Database.Entities;

    /// <summary>
    /// 
    /// </summary>
    public interface IConfigurationServiceClient
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<Configuration> GetConfiguration(String deviceIdentifier,
                                             CancellationToken cancellationToken);

        /// <summary>
        /// Posts the diagnostic logs.
        /// </summary>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="logMessages">The log messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task PostDiagnosticLogs(String deviceIdentifier, List<LogMessage> logMessages, CancellationToken cancellationToken);
    }
}
