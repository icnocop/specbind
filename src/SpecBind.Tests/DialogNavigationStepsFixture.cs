// <copyright file="DialogNavigationStepsFixture.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;

    /// <summary>
    /// Tests for Dialog Navigation Steps
    /// </summary>
    [TestClass]
    public class DialogNavigationStepsFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();

        /// <summary>
        /// Tests the WaitForDialogStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForDialogStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<DialogNavigationAction>(
                testPage.Object,
                It.Is<WaitForActionBase.WaitForActionBaseContext>(c => c.PropertyName == "mydialog")))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var steps = new DialogNavigationSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object);

            steps.WaitForDialogStep("mydialog");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the WaitForDialogWithTimeoutStep with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForDialogWithTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForDialogAction>(
                testPage.Object,
                It.Is<WaitForActionBase.WaitForActionBaseContext>(c => c.PropertyName == "mydialog" && c.Timeout.Value.TotalSeconds == 10)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var steps = new DialogNavigationSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object);

            steps.WaitForDialogWithTimeoutStep(10, "mydialog");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the WaitForDialogWithTimeoutStep using the default timeout with a successful result.
        /// </summary>
        [TestMethod]
        public void TestWaitForDialogWithDefaultTimeoutStep()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<WaitForDialogAction>(
                testPage.Object,
                It.Is<WaitForActionBase.WaitForActionBaseContext>(c => c.PropertyName == "mydialog" && c.Timeout == null)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var steps = new DialogNavigationSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object);

            steps.WaitForDialogWithTimeoutStep(0, "mydialog");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }

        /// <summary>
        /// Tests the GivenIWaitedForTheDialogToClose with a successful result.
        /// </summary>
        [TestMethod]
        public void TestGivenIWaitedForTheDialogToClose()
        {
            var testPage = new Mock<IPage>();

            var pipelineService = new Mock<IActionPipelineService>(MockBehavior.Strict);
            pipelineService.Setup(p => p.PerformAction<DialogCloseAction>(
                testPage.Object,
                It.Is<WaitForActionBase.WaitForActionBaseContext>(c => c.PropertyName == "mydialog" && c.Page == testPage.Object)))
                .Returns(ActionResult.Successful(testPage.Object));

            var scenarioContext = new Mock<IScenarioContextHelper>(MockBehavior.Strict);
            scenarioContext.Setup(s => s.GetCurrentPage()).Returns(testPage.Object);
            scenarioContext.Setup(s => s.SetCurrentPage(testPage.Object));

            var steps = new DialogNavigationSteps(scenarioContext.Object, pipelineService.Object, this.logger.Object);

            steps.GivenIWaitedForTheDialogToClose("mydialog");

            pipelineService.VerifyAll();
            scenarioContext.VerifyAll();
            testPage.VerifyAll();
        }
    }
}
