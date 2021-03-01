// <copyright file="DialogCloseActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for closing a dialog action.
    /// </summary>
    [TestClass]
    public class DialogCloseActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var dialogCloseAction = new DialogCloseAction(null, null, null);

            Assert.AreEqual("DialogCloseAction", dialogCloseAction.Name);
        }

        /// <summary>
        /// Tests waiting for the dialog to close with a dialog in the page history service succeeds.
        /// </summary>
        [TestMethod]
        public void TestCloseDialogWithDialogInPageHistoryService()
        {
            // Arrange
            var testPage = new Mock<IPage>();
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScenarioContextHelper> scenarioContextHelper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContextHelper.Setup(x => x.SetCurrentPage(testPage.Object));

            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>(MockBehavior.Strict);
            pageHistoryService.Setup(x => x.GetCurrentPage()).Returns(testPage.Object);
            pageHistoryService.Setup(x => x.FindPage("mydialog")).Returns(testPage.Object);
            pageHistoryService.Setup(x => x.Remove(testPage.Object));

            Mock<IPropertyData> propertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            propertyData.Setup(x => x.ClearCache());

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("mydialog")).Returns(propertyData.Object);

            var dialogCloseAction = new DialogCloseAction(logger.Object, scenarioContextHelper.Object, pageHistoryService.Object)
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null)
            {
                Page = testPage.Object
            };

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.EnsureOnPage(testPage.Object)).Throws<InvalidOperationException>();

            WebDriverSupport.CurrentBrowser = browser.Object;

            // Act
            var result = dialogCloseAction.Execute(context);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
            locator.VerifyAll();
            propertyData.VerifyAll();
            pageHistoryService.VerifyAll();
            scenarioContextHelper.VerifyAll();
        }

        /// <summary>
        /// Tests waiting for the dialog to close with a dialog not in the page history service fails.
        /// </summary>
        [TestMethod]
        public void TestCloseDialogWithoutDialogInPageHistoryService()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScenarioContextHelper> scenarioContextHelper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>(MockBehavior.Strict);
            pageHistoryService.Setup(x => x.FindPage("mydialog")).Returns<IPage>(null);

            var dialogCloseAction = new DialogCloseAction(logger.Object, scenarioContextHelper.Object, pageHistoryService.Object);

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null);

            // Act
            var result = dialogCloseAction.Execute(context);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.AreEqual(null, result.Result);
            Assert.AreEqual("Cannot locate a page for name: mydialog. Check page aliases in the test assembly.", result.Exception.Message);

            pageHistoryService.VerifyAll();
            scenarioContextHelper.VerifyAll();
        }
    }
}