// <copyright file="MaximizeWindowActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// A test fixture for maximizing window steps.
    /// </summary>
    [TestClass]
    public class MaximizeWindowActionFixture
    {
        /// <summary>
        /// Tests maximizing the window.
        /// </summary>
        [TestMethod]
        public void TestMaximizeWindow()
        {
            // Arrange
            var maximizeWindowAction = new MaximizeWindowAction();

            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(b => b.Maximize());

            WebDriverSupport.CurrentBrowser = browser.Object;

            // Act
            var result = maximizeWindowAction.Execute(null);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }
    }
}