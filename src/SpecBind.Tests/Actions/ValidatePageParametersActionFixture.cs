// <copyright file="ValidatePageParametersActionFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.BrowserSupport;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;
    using static SpecBind.Actions.ValidatePageParametersAction;

    /// <summary>
    /// A test fixture for the validate page parameters action
    /// </summary>
    [TestClass]
    public class ValidatePageParametersActionFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();

        /// <summary>
        /// Verifies the Name property returns the expected action name.
        /// </summary>
        [TestMethod]
        public void Name_ReturnsExpectedActionName()
        {
            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object);

            Assert.AreEqual("ValidatePageParametersAction", validatePageParametersAction.Name);
        }

        /// <summary>
        /// Verifies that executing the action using contains without a matching parameter key fails.
        /// </summary>
        [TestMethod]
        public void Execute_UsingContainsAndWithoutMatchingParameterKey_Fails()
        {
            var browser = new Mock<IBrowser>();
            browser.Setup(x => x.Url).Returns("http://example.com");

            WebDriverSupport.CurrentBrowser = browser.Object;

            Dictionary<string, string> pageParameters = new Dictionary<string, string>
            {
                { "queryStringKey", null }
            };

            ValidatePageParametersActionContext context = new ValidatePageParametersActionContext(
                PageParameterValidationAction.Contains, pageParameters);

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var result = validatePageParametersAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);
            Assert.AreEqual("Parameter key 'queryStringKey' was not found in query string ''.", result.Exception.Message);
        }

        /// <summary>
        /// Verifies that executing the action using contains without a matching parameter value fails.
        /// </summary>
        [TestMethod]
        public void Execute_UsingContainsAndWithoutMatchingParameterValue_Fails()
        {
            var browser = new Mock<IBrowser>();
            browser.Setup(x => x.Url).Returns("http://example.com?queryStringKey=queryStringValue");

            WebDriverSupport.CurrentBrowser = browser.Object;

            Dictionary<string, string> pageParameters = new Dictionary<string, string>
            {
                { "queryStringKey", null }
            };

            ValidatePageParametersActionContext context = new ValidatePageParametersActionContext(
                PageParameterValidationAction.Contains, pageParameters);

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var result = validatePageParametersAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);

            string[] expectedMessage = new[]
            {
                "Value of parameter key 'queryStringKey' does not match.",
                "Expected: <>",
                "Actual: <queryStringValue>."
            };

            Assert.AreEqual(string.Join(Environment.NewLine, expectedMessage), result.Exception.Message);
        }

        /// <summary>
        /// Verifies that executing the action using contains with a matching parameter value succeeds.
        /// </summary>
        [TestMethod]
        public void Execute_UsingContainsAndWithMatchingParameterValue_Succeeds()
        {
            var browser = new Mock<IBrowser>();
            browser.Setup(x => x.Url).Returns("http://example.com?queryStringKey=queryStringValue");

            WebDriverSupport.CurrentBrowser = browser.Object;

            Dictionary<string, string> pageParameters = new Dictionary<string, string>
            {
                { "queryStringKey", "queryStringValue" }
            };

            ValidatePageParametersActionContext context = new ValidatePageParametersActionContext(
                PageParameterValidationAction.Contains, pageParameters);

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var result = validatePageParametersAction.Execute(context);

            Assert.AreEqual(true, result.Success);
        }

        /// <summary>
        /// Verifies that executing the action using does not contain with a matching parameter key fails.
        /// </summary>
        [TestMethod]
        public void Execute_UsingDoesNotContainAndWithMatchingParameterKey_Fails()
        {
            var browser = new Mock<IBrowser>();
            browser.Setup(x => x.Url).Returns("http://example.com?queryStringKey=queryStringValue");

            WebDriverSupport.CurrentBrowser = browser.Object;

            Dictionary<string, string> pageParameters = new Dictionary<string, string>
            {
                { "queryStringKey", null }
            };

            ValidatePageParametersActionContext context = new ValidatePageParametersActionContext(
                PageParameterValidationAction.DoesNotContain, pageParameters);

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var result = validatePageParametersAction.Execute(context);

            Assert.AreEqual(false, result.Success);

            Assert.IsNotNull(result.Exception);

            Assert.AreEqual(
                $"Parameter key 'queryStringKey' was found in query string 'queryStringKey=queryStringValue'.",
                result.Exception.Message);
        }

        /// <summary>
        /// Verifies that executing the action using does not contain without a matching parameter key succeeds.
        /// </summary>
        [TestMethod]
        public void Execute_UsingDoesNotContainAndWithoutMatchingParameterKey_Succeeds()
        {
            var browser = new Mock<IBrowser>();
            browser.Setup(x => x.Url).Returns("http://example.com?");

            WebDriverSupport.CurrentBrowser = browser.Object;

            Dictionary<string, string> pageParameters = new Dictionary<string, string>
            {
                { "queryStringKey", null }
            };

            ValidatePageParametersActionContext context = new ValidatePageParametersActionContext(
                PageParameterValidationAction.DoesNotContain, pageParameters);

            Mock<IElementLocator> locator = new Mock<IElementLocator>(MockBehavior.Strict);

            var validatePageParametersAction = new ValidatePageParametersAction(this.logger.Object)
            {
                ElementLocator = locator.Object
            };

            var result = validatePageParametersAction.Execute(context);

            Assert.AreEqual(true, result.Success);
        }
    }
}