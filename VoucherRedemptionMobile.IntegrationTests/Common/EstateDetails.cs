namespace VoucherRedemptionMobile.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EstateDetails
    {
        #region Fields

        /// <summary>
        /// The merchants
        /// </summary>
        private readonly Dictionary<String, Guid> Merchants;

        /// <summary>
        /// The merchant users
        /// </summary>
        private readonly Dictionary<String, Dictionary<String, String>> MerchantUsers;

        /// <summary>
        /// The merchant users tokens
        /// </summary>
        private Dictionary<String, Dictionary<String, String>> MerchantUsersTokens;

        /// <summary>
        /// The operators
        /// </summary>
        private readonly Dictionary<String, Guid> Operators;

        private readonly Dictionary<Guid, List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)>> OperatorVouchers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EstateDetails" /> class.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="estateName">Name of the estate.</param>
        private EstateDetails(Guid estateId,
                              String estateName)
        {
            this.EstateId = estateId;
            this.EstateName = estateName;
            this.Merchants = new Dictionary<String, Guid>();
            this.Operators = new Dictionary<String, Guid>();
            this.OperatorVouchers = new Dictionary<Guid, List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public String AccessToken { get; private set; }

        /// <summary>
        /// Gets the estate identifier.
        /// </summary>
        /// <value>
        /// The estate identifier.
        /// </value>
        public Guid EstateId { get; }

        /// <summary>
        /// Gets the name of the estate.
        /// </summary>
        /// <value>
        /// The name of the estate.
        /// </value>
        public String EstateName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the operator.
        /// </summary>
        /// <param name="operatorId">The operator identifier.</param>
        /// <param name="operatorName">Name of the operator.</param>
        public void AddOperator(Guid operatorId,
                                String operatorName)
        {
            this.Operators.Add(operatorName, operatorId);
        }

        /// <summary>
        /// Adds the voucher.
        /// </summary>
        /// <param name="operatorId">The operator identifier.</param>
        /// <param name="voucherValue">The voucher value.</param>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="voucherId">The voucher identifier.</param>
        public void AddVoucher(String operatorId,
                               Decimal voucherValue,
                               Guid transactionId,
                               String voucherCode,
                               Guid voucherId,
                               DateTime expiryDateTime)
        {
            (Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime) voucherData = (transactionId, voucherValue, voucherCode, voucherId, expiryDateTime);

            KeyValuePair<String, Guid> operatorRecord = this.Operators.Single(o => o.Key == operatorId);

            if (this.OperatorVouchers.ContainsKey(operatorRecord.Value))
            {
                List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)> vouchersList = this.OperatorVouchers[operatorRecord.Value];
                vouchersList.Add(voucherData);
            }
            else
            {
                List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)> vouchers =
                    new List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)>();
                vouchers.Add(voucherData);
                this.OperatorVouchers.Add(operatorRecord.Value, vouchers);
            }
        }

        /// <summary>
        /// Creates the specified estate identifier.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="estateName">Name of the estate.</param>
        /// <returns></returns>
        public static EstateDetails Create(Guid estateId,
                                           String estateName)
        {
            return new EstateDetails(estateId, estateName);
        }

        /// <summary>
        /// Gets the operator identifier.
        /// </summary>
        /// <param name="operatorName">Name of the operator.</param>
        /// <returns></returns>
        public Guid GetOperatorId(String operatorName)
        {
            return this.Operators.Single(o => o.Key == operatorName).Value;
        }

        public (Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime) GetVoucher(Decimal voucherValue)
        {
            List<(Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime)> voucherList = this.OperatorVouchers.SelectMany(ov => ov.Value).ToList();

            (Guid transactionId, Decimal value, String voucherCode, Guid voucherId, DateTime expiryDateTime) voucherRecord = voucherList.SingleOrDefault(v => v.value == voucherValue);

            return voucherRecord;
        }

        #endregion
    }
}