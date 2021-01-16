@login @base @shared @redeemvoucher
Feature: RedeemVoucher

Background: 

	Given the following security roles exist
	| RoleName |
	| VoucherRedemption   |

	Given the following api resources exist
	| ResourceName     | DisplayName            | Secret  | Scopes           | UserClaims                 |
	| estateManagement | Estate Managememt REST | Secret1 | estateManagement | MerchantId, EstateId, role |
	| voucherManagement | Voucher Management REST | Secret1 | voucherManagement |  |
	| voucherManagementACL | Voucher Management ACL REST | Secret1 | voucherManagementACL | EstateId, role, ContractId |

	Given the following clients exist
	| ClientId         | ClientName        | Secret  | AllowedScopes                      | AllowedGrantTypes  |
	| serviceClient    | Service Client    | Secret1 | estateManagement,voucherManagement | client_credentials |
	| redemptionClient | Redemption Client | Secret1 | voucherManagementACL               | password           |

	Given I have a token to access the estate management and voucher management resources
	| ClientId      | 
	| serviceClient | 

	Given I have created the following estates
	| EstateName    |
	| Test Estate 1 |

	Given I have created the following security users
	| EmailAddress                         | Password | GivenName      | FamilyName | EstateName    | RoleName          |
	| redemptionuser@testredemption1.co.uk | 123456   | TestRedemption | User1      | Test Estate 1 | VoucherRedemption |

	Given I have created the following operators
	| EstateName    | OperatorName | RequireCustomMerchantNumber | RequireCustomTerminalNumber |
	| Test Estate 1 | Voucher      | True                        | True                        |

	When I issue the following vouchers
	| EstateName    | OperatorName | Value | TransactionId                        | RecipientEmail                 | RecipientMobile |
	| Test Estate 1 | Voucher      | 10.00 | 19f2776a-4230-40d4-8cd2-3649e18732e0 | testrecipient1@recipient.co.uk |                 |
	| Test Estate 1 | Voucher      | 20.00 | 6351e047-8f31-4472-a294-787caa5fb738 |                                | 123456788       |
	| Test Estate 1 | Voucher      | 30.00 | 7351e047-8f31-4472-a294-787caa5fb738 |                                | 123456788       |


@PRTest
Scenario: Redeem Vouchers
	Given I am on the Login Screen

	When I enter 'redemptionuser@testredemption1.co.uk' as the Email Address
	And I enter '123456' as the Password
	And I tap on Login

	Then the Home Page is displayed

	Given I tap on the Vouchers button

	Then the Vouchers Page is displayed

	Given I tap on the Redeem Voucher button

	Then the Voucher Redemption Page is displayed

	Given I enter the voucher code for the 10.00 voucher for 'Test Estate 1' 

	And I tap on the Find Voucher Button

	Then the voucher details are displayed for the 10.00 voucher for 'Test Estate 1' 

	When I tap on the Redeem Button

	Then The Voucher Redemption Successful Screen will be displayed

	