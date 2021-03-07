// <copyright file="WaitForDialogActionFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using SpecBind.Actions;
    using SpecBind.Helpers;

    /// <summary>
    /// A test fixture for the wait for dialog action.
    /// </summary>
    [TestClass]
    public class WaitForDialogActionFixture
    {
        /// <summary>
        /// Verifies that the action fails if the page history service could not find the page.
        /// </summary>
        [TestMethod]
        public void Execute_WherePageHistoryServiceCouldNotFindPage_Fails()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IPageHistoryService> pageHistoryService = new Mock<IPageHistoryService>();

            var waitForDialogAction = new WaitForDialogAction(logger.Object, null, pageHistoryService.Object);

            var context = new WaitForActionBase.WaitForActionBaseContext("mydialog", null);

            // Act
            var result = waitForDialogAction.Execute(context);

            // Assert
            Assert.AreEqual(false, result.Success);
            Assert.IsNull(result.Result);
            Assert.AreEqual("Cannot locate a page for name: mydialog. Check page aliases in the test assembly.", result.Exception.Message);

            pageHistoryService.VerifyAll();
        }
    }
}