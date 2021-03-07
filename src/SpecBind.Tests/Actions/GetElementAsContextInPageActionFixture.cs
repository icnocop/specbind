// <copyright file="GetElementAsPageActionFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Pages;

    /// <summary>
    /// A test fixture for a get element as context in page action
    /// </summary>
    [TestClass]
    public class GetElementAsContextInPageActionFixture
    {
        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var getElementAsContextInPageAction = new GetElementAsContextInPageAction();

            Assert.AreEqual("GetElementAsContextInPageAction", getElementAsContextInPageAction.Name);
        }

        /// <summary>
        /// Verifies a call to get element as context in page with a field on the page which doesn't exist throws an ElementExecuteException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void GetElementAsContextInPageAction_WithFieldWhichDoesNotExist_ThrowsElementExecuteException()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var getElementAsContextInPageAction = new GetElementAsContextInPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetElementAsContextInPageAction.GetElementAsContextInPageActionContext(page.Object, "doesnotexist");

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => getElementAsContextInPageAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Verifies a call to get element as context in page with a list element returns a failure.
        /// </summary>
        [TestMethod]
        public void GetElementAsContextInPageAction_WhenElementIsAList_ReturnsAFailure()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(true);
            propData.SetupGet(p => p.Name).Returns("MyProperty");

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var getElementAsContextInPageAction = new GetElementAsContextInPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetElementAsContextInPageAction.GetElementAsContextInPageActionContext(page.Object, "myproperty");
            var result = getElementAsContextInPageAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            StringAssert.Contains(result.Exception.Message, "MyProperty");

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Verifies a call to get element as context in page with an element that returns null returns a failure.
        /// </summary>
        [TestMethod]
        public void GetElementAsContextInPageAction_WhenElementIsNull_ReturnsAFailure()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);
            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(false);
            propData.SetupGet(p => p.Name).Returns("MyProperty");
            propData.Setup(p => p.GetItemAsPage()).Returns((IPage)null);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var getElementAsContextInPageAction = new GetElementAsContextInPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetElementAsContextInPageAction.GetElementAsContextInPageActionContext(page.Object, "myproperty");
            var result = getElementAsContextInPageAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Could not retrieve a page from property 'MyProperty'", result.Exception.Message);

            locator.VerifyAll();
            propData.VerifyAll();
        }

        /// <summary>
        /// Verifies a call to get element as context in page with an element that exists is successful.
        /// </summary>
        [TestMethod]
        public void GetElementAsContextInPageAction_WithExistingElement_Succeeds()
        {
            var page = new Mock<IPage>(MockBehavior.Strict);

            var propData = new Mock<IPropertyData>(MockBehavior.Strict);
            propData.SetupGet(p => p.IsList).Returns(false);
            propData.Setup(p => p.GetItemAsPage()).Returns(page.Object);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.GetElement("myproperty")).Returns(propData.Object);

            var getElementAsContextInPageAction = new GetElementAsContextInPageAction
            {
                ElementLocator = locator.Object
            };

            var context = new GetElementAsContextInPageAction.GetElementAsContextInPageActionContext(page.Object, "myproperty");
            var result = getElementAsContextInPageAction.Execute(context);

            Assert.AreEqual(true, result.Success);
            Assert.AreSame(page.Object, result.Result);

            locator.VerifyAll();
            propData.VerifyAll();
        }
    }
}