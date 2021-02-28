// <copyright file="BrowserWindowSteps.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Steps
{
    using System.Drawing;
    using System.Linq;
    using OpenQA.Selenium;
    using SpecBind.BrowserSupport;
    using SpecBind.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Browser Window Steps.
    /// </summary>
    [Binding]
    public class BrowserWindowSteps
    {
        private readonly BrowserFactory browserFactory;
        private readonly IBrowser browser;
        private string mainBrowserWindowHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserWindowSteps" /> class.
        /// </summary>
        /// <param name="browserFactory">The browser factory.</param>
        /// <param name="browser">The browser.</param>
        public BrowserWindowSteps(BrowserFactory browserFactory, IBrowser browser)
        {
            this.browserFactory = browserFactory;
            this.browser = browser;
        }

        /// <summary>
        /// Given I resize the browser window to a width of 600 and a height of 400.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [Given(@"I resize the browser window to a width of (.*) and a height of (.*)")]
        public void GivenIResizeTheBrowserWindowToAWidthOfAndAHeightOf(int width, int height)
        {
            SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;
            IWebDriver driver = seleniumBrowser.Driver;

            IOptions managementSettings = driver.Manage();

            managementSettings.Window.Size = new Size(width, height);
        }

        /// <summary>
        /// Given I maximize the browser window.
        /// </summary>
        [Given(@"I maximize the browser window")]
        public void GivenIMaximizeTheBrowserWindow()
        {
            SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;
            IWebDriver driver = seleniumBrowser.Driver;

            IOptions managementSettings = driver.Manage();

            managementSettings.Window.Maximize();
        }

        /// <summary>
        /// Givens the I closed the browser window.
        /// </summary>
        [Given(@"I closed the browser window")]
        public void GivenIClosedTheBrowserWindow()
        {
            WebDriverSupport.CurrentBrowser.ExecuteScript("window.close();");

            // Accept alert:
            // The webpage you are viewing is trying to close the window.
            // Do you want to close this window?
            SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;
            IWebDriver driver = seleniumBrowser.Driver;
            IAlert alert = driver.SwitchTo().Alert();
            alert.Accept();

            // switch to the previous browser window
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }

        /// <summary>
        /// Given I closed the browser.
        /// </summary>
        [Given(@"I closed the browser")]
        public void GivenIClosedTheBrowser()
        {
            this.browser.Close();

            // reset the driver for subsequent page navigation steps
            WebDriverSupport.ResetDriver();
        }

        /// <summary>
        /// Given I attached to the browser.
        /// </summary>
        [Given(@"I attached to the browser")]
        [When(@"I switched to the browser")]
        public void GivenIAttachedToTheBrowser()
        {
            // save the current browser window
            if (this.mainBrowserWindowHandle == null)
            {
                SeleniumBrowser seleniumBrowser = this.browser as SeleniumBrowser;

                this.mainBrowserWindowHandle = seleniumBrowser.GetMainBrowserWindowHandle();
            }

            // re-attach to the browser using Appium
            SeleniumBrowserFactory browserFactory = this.browserFactory as SeleniumBrowserFactory;

            browserFactory.Configuration.BrowserType = BrowserType.WinApp;

            browserFactory.Configuration.Settings.Clear();

            browserFactory.Configuration.Settings.Add("appTopLevelWindow", this.mainBrowserWindowHandle);

            IBrowser browser = browserFactory.GetBrowser();
            WebDriverSupport.CurrentBrowser = browser;

            // reset the driver
            WebDriverSupport.ResetDriver();
        }
    }
}
