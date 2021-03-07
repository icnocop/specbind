// <copyright file="WaitForPageTitleActionFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.Actions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// A test fixture for a wait for page title action
    /// </summary>
    [TestClass]
    public class WaitForPageTitleActionFixture
    {
        /// <summary>
        /// Verifies the Name property returns the expected action Name.
        /// </summary>
        [TestMethod]
        public void Name_ReturnsWaitForPageTitleAction()
        {
            var waitForPageTitleAction = new WaitForPageTitleAction();

            Assert.AreEqual("WaitForPageTitleAction", waitForPageTitleAction.Name);
        }

        /// <summary>
        /// Verifies executing the action without a matching title returns a failure.
        /// </summary>
        [TestMethod]
        public void Execute_WithoutMatchingTitle_ReturnsFailure()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(x => x.Title).Returns("DoesNotMatch");
            browser.Setup(x => x.Refresh());

            WebDriverSupport.CurrentBrowser = browser.Object;

            var action = new WaitForPageTitleAction();
            var context = new WaitForPageTitleAction.WaitForPageTitleContext("MyTitle", TimeSpan.FromSeconds(1));

            var result = action.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Page title did not contain 'MyTitle' after 00:00:01", result.Exception.Message);

            browser.VerifyAll();
        }

        /// <summary>
        /// Verifies executing the action with a matching title returns success.
        /// </summary>
        [TestMethod]
        public void Execute_WithMatchingTitle_ReturnsFailure()
        {
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(x => x.Title).Returns("Contains MyTitle");
            browser.Setup(x => x.Refresh());

            WebDriverSupport.CurrentBrowser = browser.Object;

            var action = new WaitForPageTitleAction();
            var context = new WaitForPageTitleAction.WaitForPageTitleContext("MyTitle", TimeSpan.FromSeconds(1));

            var result = action.Execute(context);

            Assert.AreEqual(true, result.Success);
        }
    }
}