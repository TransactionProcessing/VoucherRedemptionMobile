namespace VoucherRedemptionMobile.Database.Entities
{
    using System;
    using SQLite;

    /// <summary>
    /// 
    /// </summary>
    public class LogMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entry date time.
        /// </summary>
        /// <value>
        /// The entry date time.
        /// </value>
        public DateTime EntryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [PrimaryKey]
        [AutoIncrement]
        public Int32 Id { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public String LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public String Message { get; set; }

        #endregion
    }
}