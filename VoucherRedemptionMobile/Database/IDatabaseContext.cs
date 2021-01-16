namespace VoucherRedemptionMobile.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseContext
    {
        #region Methods

        /// <summary>
        /// Gets the log messages.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        Task<List<LogMessage>> GetLogMessages(Int32 batchSize);

        /// <summary>
        /// Initialises the database.
        /// </summary>
        /// <returns></returns>
        Task InitialiseDatabase();

        /// <summary>
        /// Inserts the log message.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        /// <returns></returns>
        Task InsertLogMessage(LogMessage logMessage);

        /// <summary>
        /// Inserts the log messages.
        /// </summary>
        /// <param name="logMessages">The log messages.</param>
        /// <returns></returns>
        Task InsertLogMessages(List<LogMessage> logMessages);

        /// <summary>
        /// Removes the uploaded messages.
        /// </summary>
        /// <param name="logMessagesToRemove">The log messages to remove.</param>
        /// <returns></returns>
        Task RemoveUploadedMessages(List<LogMessage> logMessagesToRemove);

        #endregion
    }
}