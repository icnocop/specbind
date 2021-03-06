﻿// <copyright file="ErrorCheckSteps.cs" company="">
//     Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind.Selenium.IntegrationTests.Steps
{
    using Actions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpecBind.ActionPipeline;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;
    using Steps = TechTalk.SpecFlow.Steps;

    /// <summary>
    /// A set of steps used to ensure errors actually occur
    /// </summary>
    [Binding]
    public class ErrorCheckSteps : Steps
    {
        private readonly DataSteps dataSteps;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCheckSteps" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="logger">The logger.</param>
        public ErrorCheckSteps(IScenarioContextHelper scenarioContext, IActionPipelineService actionPipelineService, ILogger logger)
        {
            this.dataSteps = new DataSteps(scenarioContext, actionPipelineService, logger);
        }

        /// <summary>
        /// A when step indicating that invalid data should be entered.
        /// </summary>
        /// <param name="data">The data.</param>
        [When("I enter invalid data")]
        public void WhenIEnterInvalidData(Table data)
        {
            try
            {
                this.dataSteps.WhenIEnterDataInFieldsStep(data);
            }
            catch (ElementExecuteException)
            {
                return;
            }

            throw new AssertFailedException("Step should have thrown a NoSuchElementException due to invalid data");
        }
    }
}