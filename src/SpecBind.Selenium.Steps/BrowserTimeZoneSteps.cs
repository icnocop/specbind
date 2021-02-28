// <copyright file="BrowserTimeZoneSteps.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.BrowserSupport;
    using SpecBind.Selenium;
    using SpecBind.Selenium.Drivers;
    using TechTalk.SpecFlow;

    /// <summary>
    /// An action that changes the browser's time zone
    /// </summary>
    [Binding]
    public class BrowserTimeZoneSteps
    {
        private readonly IBrowser browser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserTimeZoneSteps" /> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public BrowserTimeZoneSteps(IBrowser browser)
        {
            this.browser = browser;
        }

        /// <summary>
        /// Given the browser time zone is.
        /// </summary>
        /// <param name="timeZoneId">The time zone id.</param>
        [Given(@"the browser time zone is (.*)")]
        public void GivenTheCurrentTimezoneIs(string timeZoneId)
        {
            SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;
            IWebDriverEx driver = seleniumBrowser.Driver;

            driver.SetTimezone(timeZoneId);
        }
    }
}
