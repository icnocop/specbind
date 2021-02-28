// <copyright file="DeploymentItems.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.SampleWin32App.IntegrationTests.Features
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Deployment Items.
    /// </summary>
    [DeploymentItem(@"SpecBind.Selenium.SampleWin32App.exe")]
    [DeploymentItem("SpecBind.MsTest.Steps.dll")]
    [DeploymentItem("SpecBind.Selenium.Steps.dll")]
    public class DeploymentItems
    {
    }
}
