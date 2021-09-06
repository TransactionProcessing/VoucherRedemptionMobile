using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.WithAppium.Steps
{
    using System.Threading.Tasks;
    using Pages;
    using TechTalk.SpecFlow;

    [Binding]
    [Scope(Tag = "login")]
    public class LoginSteps
    {
        LoginPage loginPage = new LoginPage();
        MainPage mainPage = new MainPage();

        public LoginSteps()
        {
            
        }

        [Given(@"I am on the Login Screen")]
        public async Task GivenIAmOnTheLoginScreen()
        {
            await this.loginPage.AssertOnPage();
        }

        [When(@"I enter '(.*)' as the Email Address")]
        public async Task WhenIEnterAsTheEmailAddress(string emailAddress)
        {
            await this.loginPage.EnterEmailAddress(emailAddress);
        }

        [When(@"I enter '(.*)' as the Password")]
        public async Task WhenIEnterAsThePassword(String password)
        {
            await this.loginPage.EnterPassword(password);
        }

        [When(@"I tap on Login")]
        public async Task WhenITapOnLogin()
        {
            await this.loginPage.ClickLoginButton();
        }

        [Then(@"the Home Page is displayed")]
        public async Task ThenTheHomePageIsDisplayed()
        {
            await this.mainPage.AssertOnPage();
        }

    }
}
