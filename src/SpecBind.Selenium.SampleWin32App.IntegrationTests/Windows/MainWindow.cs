// <copyright file="MainWindow.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.SampleWin32App.IntegrationTests.Windows
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium.Windows;
    using Selenium;
    using SpecBind.Pages;

    /// <summary>
    /// Main Window.
    /// </summary>
    [ElementLocator(Name = "SampleWin32App")]
    [PageNavigation("SampleWin32App", IsAbsoluteUrl = true)]
    [PageAlias("Main")]
    public class MainWindow : WebElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public MainWindow(WindowsDriver<WindowsElement> driver)
            : base(driver)
        {
        }

        /// <summary>
        /// Gets or sets the display child dialog button.
        /// </summary>
        /// <value>The display child dialog button.</value>
        [ElementLocator(Name = "Display child dialog")]
        public IWebElement DisplayChildDialog { get; set; }

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>The button.</value>
        [ElementLocator(TagName = "Button", Name = "Button1")]
        public IWebElement Button1 { get; set; }

        /// <summary>
        /// Gets or sets the edit box.
        /// </summary>
        /// <value>The edit box.</value>
        [ElementLocator(TagName = "Edit")]
        public IWebElement ThisIsAnEditBox { get; set; }

        /// <summary>
        /// Gets or sets the OK button.
        /// </summary>
        /// <value>The OK button.</value>
        [ElementLocator(Name = "OK")]
        public IWebElement OK { get; set; }

        /// <summary>
        /// Gets or sets the child dialog.
        /// </summary>
        /// <value>The child dialog.</value>
        [ElementLocator(Name = "Child Dialog")]
        public ChildWindow Child { get; set; }
    }
}
