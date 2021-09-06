@background @login
Feature: Login

Background: 
	# TODO: Could add expiry date and redemption status here
	Given the following vouchers have been issued
	| VoucherCode | VoucherValue | RecipientEmail                 | RecipientMobile |
	| 0000000001  | 10.00        | testrecipient1@recipient.co.uk |                 |
	| 0000000002  | 20.00        | testrecipient2@recipient.co.uk |                 |
	| 0000000003  | 10.00        |                                | 123456789       |
	| 0000000004  | 20.00        |                                | 123456788       |

	Given the application in in test mode

@PRTest	
Scenario: Login
	Given I am on the Login Screen

	When I enter 'redemptionuser@testredemption1.co.uk' as the Email Address
	And I enter '123456' as the Password
	And I tap on Login

	Then the Home Page is displayed