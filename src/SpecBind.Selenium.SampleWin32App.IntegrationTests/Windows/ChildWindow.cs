// <copyright file="ChildWindow.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.SampleWin32App.IntegrationTests.Windows
{
    using System;
    using System.Collections.Generic;
    using OpenQA.Selenium;
    using Selenium;
    using SpecBind.Pages;

    /// <summary>
    /// Child Window.
    /// </summary>
    [PageNavigation("Child Dialog", IsAbsoluteUrl = true)]
    public class ChildWindow : WebElement, ILocationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildWindow"/> class.
        /// </summary>
        /// <param name="searchContext">The driver used to search for elements.</param>
        public ChildWindow(ISearchContext searchContext)
            : base(searchContext)
        {
        }

        /// <summary>
        /// Gets or sets the OK button.
        /// </summary>
        /// <value>The OK button.</value>
        [ElementLocator(Name = "OK")]
        public IWebElement OK { get; set; }

        /// <summary>
        /// Gets the page location.
        /// </summary>
        /// <returns>A collection of URIs to validate.</returns>
        public IList<string> GetPageLocation()
        {
            try
            {
                string title = this.GetAttribute("Name");
                return new[] { title };
            }
            catch (NoSuchElementException ex)
            {
                // Could not find element by: ...
                Console.WriteLine(ex.Message);
                return new string[] { };
            }
        }
    }
}