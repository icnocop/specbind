// <copyright file="WaitForElementsActionFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;

    /// <summary>
    /// A test fixture for a wait for elements action
    /// </summary>
    [TestClass]
    public class WaitForElementsActionFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();

        /// <summary>
        /// Tests getting the name of the action.
        /// </summary>
        [TestMethod]
        public void TestGetActionName()
        {
            var waitForElementsAction = new WaitForElementsAction(this.logger.Object);

            Assert.AreEqual("WaitForElementsAction", waitForElementsAction.Name);
        }

        /// <summary>
        /// Tests the action execute with a field on the page that doesn't exist.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ElementExecuteException))]
        public void TestExecuteFieldDoesNotExist()
        {
            var criteriaTable = new Table("Field", "Rule", "Value");
            var validationTable = criteriaTable.ToValidationTable();

            var page = new Mock<IPage>(MockBehavior.Strict);

            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            IPropertyData expectedPropertyData = null;
            locator.Setup(p => p.TryGetElement("doesnotexist", out expectedPropertyData)).Returns(false);
            locator.Setup(p => p.GetElement("doesnotexist")).Throws(new ElementExecuteException("Cannot find item"));

            var waitForElementsAction = new WaitForElementsAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementsAction.WaitForElementsContext(page.Object, validationTable, TimeSpan.FromMilliseconds(1));

            ExceptionHelper.SetupForException<ElementExecuteException>(
                () => waitForElementsAction.Execute(context), e => locator.VerifyAll());
        }

        /// <summary>
        /// Tests the element execute success.
        /// </summary>
        [TestMethod]
        public void TestExecuteSuccess()
        {
            var criteriaTable = new Table("Field", "Rule", "Value");
            criteriaTable.AddRow("myproperty", "Equals", "mypropertyvalue");
            var validationTable = criteriaTable.ToValidationTable();

            var page = new Mock<IPage>(MockBehavior.Strict);

            var timeout = TimeSpan.FromMilliseconds(1);

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            string actualValue;
            mockPropertyData.Setup(x => x.ValidateItem(validationTable.Validations.Single(), out actualValue)).Returns(true);

            IPropertyData propertyData = null;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty(null, out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            var WaitForElementsAction = new WaitForElementsAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementsAction.WaitForElementsContext(page.Object, validationTable, timeout);
            var result = WaitForElementsAction.Execute(context);

            Assert.AreEqual(true, result.Success);

            locator.VerifyAll();
            mockPropertyData.VerifyAll();
            page.VerifyAll();
        }

        // Define a delegate with the params of the method that returns void.
        private delegate void TryGetPropertyMethodDelegate(string propertyName, out IPropertyData output);

        /// <summary>
        /// Tests the element execute success.
        /// </summary>
        [TestMethod]
        public void TestExecuteFailureIfResultIsFalse()
        {
            var criteriaTable = new Table("Field", "Rule", "Value");
            criteriaTable.AddRow("myproperty", "Equals", "mypropertyvalue");
            var validationTable = criteriaTable.ToValidationTable();

            var page = new Mock<IPage>(MockBehavior.Strict);

            var timeout = TimeSpan.FromSeconds(1);

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>(MockBehavior.Strict);
            string actualValue;
            mockPropertyData.Setup(x => x.ValidateItem(validationTable.Validations.Single(), out actualValue)).Returns(false);

            IPropertyData propertyData = null;
            var locator = new Mock<IElementLocator>(MockBehavior.Strict);
            locator.Setup(p => p.TryGetProperty(null, out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            var WaitForElementsAction = new WaitForElementsAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var context = new WaitForElementsAction.WaitForElementsContext(page.Object, validationTable, timeout);
            var result = WaitForElementsAction.Execute(context);

            Assert.AreEqual(false, result.Success);
            Assert.AreEqual("Value comparison(s) failed after 00:00:01.", result.Exception.Message);

            locator.VerifyAll();
            mockPropertyData.VerifyAll();
            page.VerifyAll();
        }
    }
}