// <copyright file="TokenStepsFixture.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>
namespace SpecBind.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;
    using static SpecBind.TokenSteps;

    /// <summary>
    /// A test fixture for the token steps class.
    /// </summary>
    [TestClass]
    public class TokenStepsFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();
        private readonly Mock<ITokenManager> tokenManager = new Mock<ITokenManager>();

        /// <summary>
        /// Tests the SetTokenFromFieldStep method pulls the value from the field and sets the value.
        /// </summary>
        [TestMethod]
        public void TestSetTokenFromFieldStepSetsCurrentValue()
        {
            var page = new Mock<IPage>();
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<SetTokenFromValueAction>(
                    page.Object,
                    It.Is<SetTokenFromValueAction.TokenFieldContext>(c => c.TokenName == "MyToken" && c.PropertyName == "somefield")))
                    .Returns(ActionResult.Successful("The Field Value"));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(page.Object);

            var steps = new TokenSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object, this.tokenManager.Object);

            steps.SetTokenFromFieldStep("MyToken", "SomeField");

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Tests the SetTokenFromFieldStep method pulls the value from the field and sets the value.
        /// </summary>
        [TestMethod]
        public void TestValidateTokenStep()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(
                p => p.PerformAction<ValidateTokenAction>(
                    null,
                    It.Is<ValidateTokenAction.ValidateTokenActionContext>(
                    c => c.ValidationTable.ValidationCount == 1 &&
                         c.ValidationTable.Validations.First().RawFieldName == "My Token" &&
                         c.ValidationTable.Validations.First().RawComparisonType == "Equals" &&
                         c.ValidationTable.Validations.First().RawComparisonValue == "test")))
                    .Returns(ActionResult.Successful());

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            var steps = new TokenSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object, this.tokenManager.Object);

            steps.ValidateTokenValueStep("My Token", "Equals ", " test");

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
        }

        /// <summary>
        /// Verifies that calling SetTheFollowingTokens calls TokenManager.SetToken.
        /// </summary>
        [TestMethod]
        public void SetTheFollowingTokens()
        {
            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);

            Mock<ITokenManager> tokenManager = new Mock<ITokenManager>();
            tokenManager.Setup(x => x.SetToken("MyToken", "SomeField"));

            var steps = new TokenSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object, tokenManager.Object);

            steps.SetTheFollowingTokens(new List<Token> { new Token { Name = "MyToken", Value = "SomeField" } });

            scenarioContext.VerifyAll();
            pipelineService.VerifyAll();
            tokenManager.VerifyAll();
        }

        /// <summary>
        /// Verifies that calling Transform creates the expected token.
        /// </summary>
        [TestMethod]
        public void Transform()
        {
            var steps = new TokenSteps(null, null, null, null);

            Table table = new Table("Name", "Value");
            table.AddRow("MyToken", "SomeField");

            IEnumerable<Token> transformed = steps.Transform(table);
            Assert.AreEqual(1, transformed.Count());
            Assert.AreEqual("MyToken", transformed.Single().Name);
            Assert.AreEqual("SomeField", transformed.Single().Value);
        }
    }
}