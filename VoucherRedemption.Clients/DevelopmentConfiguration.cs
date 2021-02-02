namespace VoucherRedemption.Clients
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IConfiguration" />
    [ExcludeFromCodeCoverage]
    public class DevelopmentConfiguration : IConfiguration
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DevelopmentConfiguration"/> class.
        /// </summary>
        public DevelopmentConfiguration()
        {
            this.VoucherManagementACL = "http://192.168.1.133:5008";
            this.EstateManagement = "http://192.168.1.133:5000";
            this.SecurityService = "http://192.168.1.133:5001";
            this.ClientId = "mobileAppClient";
            this.ClientSecret = "d192cbc46d834d0da90e8a9d50ded543";
            this.LogLevel = LogLevel.Debug;
            this.EnableAutoUpdates = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public String ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public String ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the security service.
        /// </summary>
        /// <value>
        /// The security service.
        /// </value>
        public String SecurityService { get; set; }

        public String VoucherManagementACL { get; set; }

        public String EstateManagement { get; set; }

        public LogLevel LogLevel { get; set; }

        public Boolean EnableAutoUpdates { get; set; }

        #endregion
    }
}