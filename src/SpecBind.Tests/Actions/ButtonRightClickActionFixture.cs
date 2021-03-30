// <copyright file="ButtonRightClickActionFixture.cs">
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
    /// A test fixture for a button right click action
    /// </summary>
    [TestClass]
    public class ButtonRightClickActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var buttonRightClickAction = new ButtonRightClickAction();

            Assert.AreEqual("ButtonRightClickAction", buttonRightClickAction.Name);
        }

        /// <summary>
        ///     Tests the button right click action with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestRightClickItemFieldDoesNotExist()
        {
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var buttonRightClickAction = new ButtonRightClickAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("doesnotexist");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => buttonRightClickAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        ///     Tests the button right click action with an element that exists and can be clicked.
        /// </summary>
        [TestMethod]
        public void TestRightClickItemSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.NotMoving, null)).Returns(true);
            propData.Setup(p => p.WaitForElementCondition(WaitConditions.BecomesEnabled, null)).Returns(true);
            propData.Setup(p => p.RightClickElement());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var buttonRightClickAction = new ButtonRightClickAction
            {
                ElementLocator = locator.Object
            };

            var context = new ActionContext("myproperty");
            var result = buttonRightClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
		///     Tests the button right click action with an element that exists and can be clicked.
		/// </summary>
		[TestMethod]
        public void TestRightClickItemWhenWaitIsEnabledReturnsSuccess()
        {
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.Setup(p => p.RightClickElement());

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            ButtonRightClickAction.WaitForStillElementBeforeClicking = false;

            var buttonRightClickAction = new ButtonRightClickAction
            {
                ElementLocator = locator.Object

            };

            var context = new ActionContext("myproperty");
            var result = buttonRightClickAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}