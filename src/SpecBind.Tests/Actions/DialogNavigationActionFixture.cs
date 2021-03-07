// <copyright file="DialogNavigationActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for dialog navigation steps.
    /// </summary>
    [TestClass]
    public class DialogNavigationActionFixture
    {
        /// <summary>
        /// Tests navigating to a dialog that doesn't exist fails.
        /// </summary>
        [TestMethod]
        public void TestNavigateToDialogWhichDoesNotExist()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>();

            Dictionary<Type, IPage> pageHistory = new Dictionary<Type, IPage>();
            pageHistoryService.Setup(x => x.PageHistory).Returns(pageHistory);

            var dialogNavigationAction = new DialogNavigationAction(logger.Object, null, pageHistoryService.Object);

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null);

            // Act
            var result = dialogNavigationAction.Execute(context);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.IsNull(result.Result);
            Assert.AreEqual($"A property with the name 'mydialog' was not found in any of the displayed pages:{Environment.NewLine}", result.Exception.Message);

            pageHistoryService.VerifyAll();
        }

        /// <summary>
        /// Tests navigating to a dialog that exist succeeds.
        /// </summary>
        [TestMethod]
        public void TestNavigateToDialog()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();

            Mock<IPage> page = new Mock<IPage>();
            Mock<IScenarioContextHelper> scenarioContextHelper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContextHelper.Setup(x => x.SetCurrentPage(page.Object));

            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>();
            pageHistoryService.Setup(x => x.FindPage("mydialog")).Returns(page.Object);

            var dialogNavigationAction = new DialogNavigationAction(logger.Object, scenarioContextHelper.Object, pageHistoryService.Object);

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.EnsureOnPage(page.Object));

            WebDriverSupport.CurrentBrowser = browser.Object;

            // Act
            var result = dialogNavigationAction.Execute(context);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
            pageHistoryService.VerifyAll();
            scenarioContextHelper.VerifyAll();
        }

        /// <summary>
        /// Verifies that the action fails if the page history service could not find the page.
        /// </summary>
        [TestMethod]
        public void Execute_WherePageHistoryServiceCouldNotFindPage_Fails()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();

            Mock<IPage> page = new Mock<IPage>();
            Mock<IScenarioContextHelper> scenarioContextHelper = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>();
            pageHistoryService.Setup(x => x.FindPage("mydialog")).Returns<IPage>(null);

            var dialogNavigationAction = new DialogNavigationAction(logger.Object, scenarioContextHelper.Object, pageHistoryService.Object);

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null);

            var browser = new Mock<IBrowser>(MockBehavior.Strict);

            WebDriverSupport.CurrentBrowser = browser.Object;

            // Act
            var result = dialogNavigationAction.Execute(context);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("A property with the name 'mydialog' was not found in any of the displayed pages:", result.Exception.Message);

            browser.VerifyAll();
            pageHistoryService.VerifyAll();
            scenarioContextHelper.VerifyAll();
        }
    }
}