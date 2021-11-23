using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.TestClients
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using SecurityService.Client;
    using SecurityService.DataTransferObjects;
    using SecurityService.DataTransferObjects.Requests;
    using SecurityService.DataTransferObjects.Responses;
    using VoucherManagementACL.DataTransferObjects.Responses;
    using VoucherRedemption.Clients;
    
    public class TestConfigurationServiceClient : IConfigurationServiceClient
    {
        public async Task<Configuration> GetConfiguration(String deviceIdentifier,
                                                          CancellationToken cancellationToken)
        {
            return new Configuration
                   {
                       EnableAutoUpdates = false,
                       //LogLevel = LogLevel.Debug,
                   };
        }

        public async Task PostDiagnosticLogs(String deviceIdentifier,
                                             List<LogMessage> logMessages,
                                             CancellationToken cancellationToken)
        {
            return;
        }
    }

    public class TestSecurityServiceClient : ISecurityServiceClient
    {
        private List<(UserDetails userDetails, String password)> Users = new List<(UserDetails userDetails, String password)>();

        public (UserDetails userDetails, String password) CreateUserDetails(String userName, String password = "123456")
        {
            var userDetails = new UserDetails
                    {
                        Claims = new Dictionary<String, String>(),
                        EmailAddress =userName,
                        PhoneNumber = String.Empty,
                        Roles = new List<String>(),
                        UserId = Guid.NewGuid(),
                        UserName = userName
                    };
            this.Users.Add((userDetails, password));

            return (userDetails, password);
        }

        public async Task<TokenResponse> GetToken(String username,
                                                  String password,
                                                  String clientId,
                                                  String clientSecret,
                                                  CancellationToken cancellationToken)
        {
            (UserDetails userDetails, String password) user = this.Users.SingleOrDefault(u => u.userDetails.EmailAddress == username && u.password == password);
            if (user.userDetails == null)
            {
                throw new Exception($"User {username} not found");
            }
            return TokenResponse.Create("token", null, 0, DateTimeOffset.Now);
        }

        #region Not Implemented
        public async Task<CreateApiResourceResponse> CreateApiResource(CreateApiResourceRequest createApiResourceRequest,
                                                                       CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<CreateApiScopeResponse> CreateApiScope(CreateApiScopeRequest createApiScopeRequest,
                                                                 CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<CreateClientResponse> CreateClient(CreateClientRequest createClientRequest,
                                                             CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<CreateIdentityResourceResponse> CreateIdentityResource(CreateIdentityResourceRequest createIdentityResourceRequest,
                                                                                 CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest createRoleRequest,
                                                         CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest createUserRequest,
                                                         CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<ApiResourceDetails> GetApiResource(String apiResourceName,
                                                             CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<ApiScopeDetails> GetApiScope(String apiScopeName,
                                                       CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<ApiResourceDetails>> GetApiResources(CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<ApiScopeDetails>> GetApiScopes(CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<ClientDetails> GetClient(String clientId,
                                                   CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<ClientDetails>> GetClients(CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<IdentityResourceDetails> GetIdentityResource(String identityResourceName,
                                                                       CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<IdentityResourceDetails>> GetIdentityResources(CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<RoleDetails> GetRole(Guid roleId,
                                               CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<RoleDetails>> GetRoles(CancellationToken cancellationToken)
        {
            return null;
        }
        
        public async Task<TokenResponse> GetToken(String clientId,
                                                  String clientSecret,
                                                  CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<TokenResponse> GetToken(String clientId,
                                                  String clientSecret,
                                                  String refreshToken,
                                                  CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<UserDetails> GetUser(Guid userId,
                                               CancellationToken cancellationToken)
        {
            return null;
        }

        public async Task<List<UserDetails>> GetUsers(String userName,
                                                      CancellationToken cancellationToken)
        {
            return null;
        }
        #endregion
    }

    public class TestVoucherManagementACLClient : IVoucherManagerACLClient
    {
        public List<Voucher> Vouchers = new List<Voucher>();

        public TestVoucherManagementACLClient()
        {
        }

        public void CreateTestVoucher(String voucherCode, Decimal value, String recipientEmail = null, String recipientMobile = null)
        {
            Voucher voucher = new Voucher
                              {
                                  Balance = value,
                                  Barcode = String.Empty, // TODO: Generate a barcode
                                  EstateId = Guid.Parse("347C8CD4-A194-4115-A36F-A75A5E24C49B"),
                                  ContractId = Guid.Parse("FC3E5F36-54AA-4BA2-8BF8-5391ACE4BD4B"),
                                  ExpiryDate = DateTime.Now.AddDays(30),
                                  GeneratedDateTime = DateTime.Now,
                                  IsGenerated = true,
                                  IsIssued = true,
                                  IsRedeemed = false,
                                  IssuedDateTime = DateTime.Now.AddSeconds(5),
                                  Message = String.Empty,
                                  RecipientEmail = recipientEmail,
                                  RecipientMobile = recipientMobile,
                                  RedeemedDateTime = DateTime.MinValue,
                                  TransactionId = Guid.NewGuid(),
                                  Value = value,
                                  VoucherCode = voucherCode,
                                  VoucherId = Guid.NewGuid()
                              };
            this.Vouchers.Add(voucher);
        }

        public async Task<GetVoucherResponseMessage> GetVoucher(String accessToken,
                                                                String applicationVersion,
                                                                String voucherCode,
                                                                CancellationToken cancellationToken)
        {
            var voucher = this.Vouchers.SingleOrDefault(v => v.VoucherCode == voucherCode);

            if (voucher == null)
            {
                // TODO: Check the response here
                return null;
            }

            return new GetVoucherResponseMessage
                   {
                       VoucherId = voucher.VoucherId,
                       VoucherCode = voucher.VoucherCode,
                       EstateId = voucher.EstateId,
                       ExpiryDate = voucher.ExpiryDate,
                       Value = voucher.Value,
                       ContractId = voucher.ContractId,
                       ResponseCode = "0000",
                       ResponseMessage = "SUCCESS"
                   };
        }

        public async Task<RedeemVoucherResponseMessage> RedeemVoucher(String accessToken,
                                                                      String applicationVersion,
                                                                      String voucherCode,
                                                                      CancellationToken cancellationToken)
        {
            var voucher = this.Vouchers.SingleOrDefault(v => v.VoucherCode == voucherCode);

            if (voucher == null)
            {
                // TODO: Check the response here
                return null;
            }

            voucher.IsRedeemed = true;
            voucher.RedeemedDateTime = DateTime.Now;

            return new RedeemVoucherResponseMessage
                   {
                       ContractId = voucher.ContractId,
                       ResponseMessage = "SUCCESS",
                       VoucherCode = voucher.VoucherCode,
                       EstateId = voucher.EstateId,
                       ExpiryDate = voucher.ExpiryDate,
                       Balance = 0,
                       ResponseCode = "0000"
                   };
        }
    }
}
