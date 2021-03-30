// <copyright file="ButtonDoubleClickActionFixture.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a button double click action
    /// </summary>
    [TestClass]
    public class ButtonDoubleClickActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonDoubleClickAction = new ButtonDoubleClickAction();

            Assert.AreEqual("ButtonDoubleClickAction", buttonDoubleClickAction.Name);
        }

        /// <summary>
        ///     Tests the button double click action with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestDoubleClickItemFieldDoesNotExist()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonDoubleClickAction = new ButtonDoubleClickAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("doesnotexist");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonDoubleClickAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        ///     Tests the button double click action with an element that exists and can be clicked.
        /// </summary>
        [TestMethod]
        public void TestDoubleClickItemSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.NotMoving, null)).Returns(true);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.BecomesEnabled, null)).Returns(true);
            propData.Setup(p => p.DoubleClickElement());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonDoubleClickAction = new ButtonDoubleClickAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = buttonDoubleClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
		///     Tests the button double click action with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
        public void TestDoubleClickItemWhenWaitIsEnabledReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.DoubleClickElement());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            ButtonDoubleClickAction.WaitForStillElementBeforeClicking = false;

            var buttonDoubleClickAction = new ButtonDoubleClickAction
            {
                ElementLocator = locator.Object

            };

            var context = new ActionContext("myproperty");
            var result = buttonDoubleClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}