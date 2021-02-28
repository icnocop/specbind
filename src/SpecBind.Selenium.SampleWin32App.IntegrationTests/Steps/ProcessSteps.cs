// <copyright file="ProcessSteps.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.SampleWin32App.IntegrationTests.Steps
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using BrowserSupport;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Process Steps.
    /// </summary>
    [Binding]
    public class ProcessSteps
    {
        private readonly BrowserFactory browserFactory;
        private readonly ScenarioContext scenarioContext;
        private Process process;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSteps" /> class.
        /// </summary>
        /// <param name="browserFactory">The browser factory.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        public ProcessSteps(BrowserFactory browserFactory, ScenarioContext scenarioContext)
        {
            this.browserFactory = browserFactory;
            this.scenarioContext = scenarioContext;
        }

        /// <summary>
        /// Runs after each scenario.
        /// </summary>
        [After]
        public void After()
        {
            try
            {
                if (this.scenarioContext.ScenarioContainer.IsRegistered<IBrowser>())
                {
                    IBrowser application = this.scenarioContext.ScenarioContainer.Resolve<IBrowser>();
                    application.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Given I launched the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"I launched the file ""(.*)""")]
        public void GivenILaunchedTheFile(string fileName)
        {
            this.process = Process.Start(fileName);
        }

        /// <summary>
        /// Given I launched the application.
        /// </summary>
        [Given(@"I launched the application")]
        public void GivenILaunchedTheApplication()
        {
            SeleniumBrowserFactory browserFactory = this.browserFactory as SeleniumBrowserFactory;

            string exeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SpecBind.Selenium.SampleWin32App.exe");

            browserFactory.Configuration.Settings.Clear();
            browserFactory.Configuration.Settings.Add("app", exeFilePath);
        }

        /// <summary>
        /// Given I attached to the application.
        /// </summary>
        [Given(@"I attached to the application")]
        public void GivenIAttachedToTheProcess()
        {
            string mainWindowHandle = this.WaitForMainWindow(this.process);
            if (mainWindowHandle == null)
            {
                throw new Exception("Could not find main window of process.");
            }

            SeleniumBrowserFactory browserFactory = this.browserFactory as SeleniumBrowserFactory;

            browserFactory.Configuration.Settings.Clear();
            browserFactory.Configuration.Settings.Add("appTopLevelWindow", mainWindowHandle);
        }

        /// <summary>
        /// Then the application should exit with code 0.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        [Then(@"the application should exit with code (.*)")]
        public void ThenTheApplicationShouldExitWithCode(int exitCode)
        {
            Assert.IsTrue(this.process.HasExited);

            Assert.AreEqual(exitCode, this.process.ExitCode);
        }

        private string WaitForMainWindow(Process process)
        {
            process.Refresh();

            while ((!process.HasExited)
                && (process.MainWindowHandle == IntPtr.Zero))
            {
                Thread.Sleep(100);
                process.Refresh();
            }

            if (process.HasExited)
            {
                return null;
            }

            // convert main window handle to hex
            return process.MainWindowHandle.ToInt32().ToString("x");
        }
    }
}
