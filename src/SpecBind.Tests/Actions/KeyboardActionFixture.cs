// <copyright file="KeyboardActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// A test fixture for keyboard action.
    /// </summary>
    [TestClass]
    public class KeyboardActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var keyboardAction = new KeyboardAction(null);

            Assert.AreEqual("KeyboardAction", keyboardAction.Name);
        }

        /// <summary>
        /// Tests sending keys.
        /// </summary>
        [TestMethod]
        public void TestSendKeys()
        {
            // Arrange
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(x => x.SendKeys("abc"));

            var keyboardAction = new KeyboardAction(browser.Object);

            var context = new KeyboardAction.KeyboardActionContext("abc", KeyboardAction.KeyboardSendType.Default);

            // Act
            var result = keyboardAction.Execute(context);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests pressing keys.
        /// </summary>
        [TestMethod]
        public void TestPressKeys()
        {
            // Arrange
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(x => x.PressKeys("abc"));

            var keyboardAction = new KeyboardAction(browser.Object);

            var context = new KeyboardAction.KeyboardActionContext("abc", KeyboardAction.KeyboardSendType.Press);

            // Act
            var result = keyboardAction.Execute(context);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }

        /// <summary>
        /// Tests releasing keys.
        /// </summary>
        [TestMethod]
        public void TestReleaseKeys()
        {
            // Arrange
            var browser = new Mock<IBrowser>(MockBehavior.Strict);
            browser.Setup(x => x.ReleaseKeys("abc"));

            var keyboardAction = new KeyboardAction(browser.Object);

            var context = new KeyboardAction.KeyboardActionContext("abc", KeyboardAction.KeyboardSendType.Release);

            // Act
            var result = keyboardAction.Execute(context);

            // Assert
            Assert.AreEqual(true, result.Success);

            browser.VerifyAll();
        }
    }
}