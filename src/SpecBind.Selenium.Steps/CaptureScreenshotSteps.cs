// <copyright file="CaptureScreenshotSteps.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Steps
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.Extensions;
    using SpecBind.BrowserSupport;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using SpecBind.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Capture Screenshot Steps.
    /// </summary>
    [Binding]
    public class CaptureScreenshotSteps
    {
        private readonly IScenarioContextHelper scenarioContext;
        private readonly TestContext testContext;
        private string outputDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureScreenshotSteps" /> class.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="testContext">The test context.</param>
        public CaptureScreenshotSteps(
            IScenarioContextHelper scenarioContext,
            TestContext testContext)
        {
            this.scenarioContext = scenarioContext;
            this.testContext = testContext;
            this.outputDirectory = this.testContext.TestRunResultsDirectory;
        }

        private IWebDriver Driver => (WebDriverSupport.CurrentBrowser as SeleniumBase).Driver;

        /// <summary>
        /// Given the screenshot output directory "output directory".
        /// </summary>
        /// <param name="outputDirectory">The output directory.</param>
        [Given(@"the screenshot output directory ""(.*)""")]
        public void GivenTheScreenshotOutputDirectory(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }

        /// <summary>
        /// Given a screenshot is captured of "element" as "filename.png".
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"a screenshot is captured of ""(.*)"" as ""(.*)""")]
        [When(@"a screenshot is captured of ""(.*)"" as ""(.*)""")]
        public void GivenAScreenshotIsCapturedOfAs(string fieldName, string fileName)
        {
            this.GivenAScreenshotIsCapturedOfAsCroppedToPixelsWide(fieldName, fileName, -1);
        }

        /// <summary>
        /// Given a screenshot is captured of "element" as "filename.png" cropped to 100 pixels wide.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="width">The width.</param>
        [Given(@"a screenshot is captured of ""(.*)"" as ""(.*)"" cropped to (.*) pixels wide")]
        public void GivenAScreenshotIsCapturedOfAsCroppedToPixelsWide(string fieldName, string fileName, int width)
        {
            var currentPage = this.GetPageFromContext();

            IPropertyData propertyData;
            bool elementExists = currentPage.TryGetElement(fieldName.ToLookupKey(), out propertyData);
            Assert.IsTrue(elementExists, $"Element mapped to property '{fieldName.ToLookupKey()}' does not exist on page {currentPage.PageType.Name}");

            var nativePage = currentPage.GetNativePage();

            PropertyInfo propertyInfo = currentPage.PageType.GetProperty(propertyData.Name);

            WebElement element = propertyInfo.GetValue(nativePage, null) as WebElement;
            Assert.IsNotNull(element, $"Element '{fieldName.ToLookupKey()}' on page '{currentPage.PageType.Name}' is not a WebElement");

            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            driver.TakeScreenshot(element, filePath, width == -1 ? (int?)null : width);

            this.testContext.AddResultFile(filePath);
        }

        /// <summary>
        /// Given a screenshot is captured as "filename.png".
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"a screenshot is captured as ""(.*)""")]
        public void GivenAScreenshotIsCapturedAs(string fileName)
        {
            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            driver.TakeScreenshot(filePath);

            this.testContext.AddResultFile(filePath);
        }

        /// <summary>
        /// Given a screenshot is captured of the current window as.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"a screenshot is captured of the current window as ""(.*)""")]
        public void GivenAScreenshotIsCapturedOfTheCurrentWindowAs(string fileName)
        {
            IWebDriver driver = this.Driver;

            if (!Directory.Exists(this.outputDirectory))
            {
                Directory.CreateDirectory(this.outputDirectory);
            }

            string filePath = Path.Combine(this.outputDirectory, fileName);

            string fileExtension = Path.GetExtension(filePath).Replace(".", string.Empty);
            ImageFormat format = IWebDriverExtensions.KnownImageFormats[fileExtension.ToLower()];
            ScreenshotImageFormat imageFormat = IWebDriverExtensions.KnownScreenshotImageFormats[fileExtension.ToLower()];

            var screenshot = driver.TakeScreenshot();
            screenshot.SaveAsFile(filePath, imageFormat);
        }

        /// <summary>
        /// Given a screenshot is captured of the current dialog as.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"a screenshot is captured of the current dialog as ""(.*)""")]
        public void GivenAScreenshotIsCapturedOfTheCurrentDialogAs(string fileName)
        {
            var currentPage = this.GetPageFromContext();

            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            driver.TakeScreenshot(currentPage.GetNativePage<WebElement>(), filePath);
        }

        /// <summary>
        /// Givens a native screenshot is captured of the current dialog as "file name".
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        [Given(@"a native screenshot is captured of the current dialog as ""(.*)""")]
        public void GivenANativeScreenshotIsCapturedRelativeToAs(string fileName)
        {
            Rectangle elementRect = this.GetCurrentElementRectangle();

            Rectangle offsetCrop = new Rectangle(0, 0, elementRect.Width, elementRect.Height);

            this.CaptureNativeScreenshotRelativeToElement(elementRect, fileName, offsetCrop);
        }

        /// <summary>
        /// Given a screenshot is captured of the current window as "file name" cropped to x pixels wide and y pixels high.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [Given(@"a screenshot is captured of the current window as ""(.*)"" cropped to (.*) pixels wide and (.*) pixels high")]
        public void GivenAScreenshotIsCapturedOfTheCurrentWindowAsCroppedToPixelsWideAndPixelsHigh(string fileName, int width, int height)
        {
            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            driver.TakeScreenshot(filePath, width, height);
        }

        /// <summary>
        /// Given a screenshot is captured relative to "element" as "file name".
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="offsetCrop">The offset crop.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">element - Cropping rectangle is out of range.</exception>
        [Given(@"a screenshot is captured relative to ""(.*)"" as ""(.*)""")]
        public void GivenAScreenshotIsCapturedRelativeToAs(string fieldName, string fileName, Rectangle offsetCrop)
        {
            Rectangle elementRect = this.GetElementRectangle(fieldName);

            this.CaptureScreenshotRelativeToElement(elementRect, fileName, offsetCrop);
        }

        /// <summary>
        /// Givens a native screenshot is captured relative to as.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="offsetCrop">The offset crop.</param>
        [Given(@"a native screenshot is captured relative to ""(.*)"" as ""(.*)""")]
        public void GivenANativeScreenshotIsCapturedRelativeToAs(string fieldName, string fileName, Rectangle offsetCrop)
        {
            Rectangle elementRect = this.GetElementRectangle(fieldName);

            this.CaptureNativeScreenshotRelativeToElement(elementRect, fileName, offsetCrop);
        }

        private void CaptureScreenshot(string fileName)
        {
            string filePath = Path.Combine(this.outputDirectory, fileName);
            ImageFormat format = this.GetImageFormat(filePath);

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                bitmap.Save(filePath, format);
            }
        }

        private void CaptureScreenshotRelativeToElement(Rectangle elementRect, string fileName, Rectangle offsetCrop)
        {
            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            ImageFormat format = this.GetImageFormat(filePath);

            Screenshot screenShot = (driver as ITakesScreenshot).GetScreenshot();

            using (MemoryStream memoryStream = new MemoryStream(screenShot.AsByteArray))
            {
                using (Bitmap bitmap = new Bitmap(memoryStream))
                {
                    elementRect.X += offsetCrop.X;
                    elementRect.Y += offsetCrop.Y;
                    elementRect.Width = offsetCrop.Width;
                    elementRect.Height = offsetCrop.Height;

                    elementRect.Intersect(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                    if (elementRect.IsEmpty)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offsetCrop), "Cropping rectangle is out of range.");
                    }

                    using (Bitmap clone = bitmap.Clone(
                        elementRect,
                        bitmap.PixelFormat))
                    {
                        clone.Save(filePath, format);
                    }
                }
            }
        }

        private void CaptureNativeScreenshotRelativeToElement(Rectangle elementRect, string fileName, Rectangle offsetCrop)
        {
            IWebDriver driver = this.Driver;

            string filePath = Path.Combine(this.outputDirectory, fileName);

            ImageFormat format = this.GetImageFormat(filePath);

            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                elementRect.X += driver.Manage().Window.Position.X + offsetCrop.X;
                elementRect.Y += driver.Manage().Window.Position.Y + offsetCrop.Y;
                elementRect.Width = offsetCrop.Width;
                elementRect.Height = offsetCrop.Height;

                elementRect.Intersect(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                if (elementRect.IsEmpty)
                {
                    throw new ArgumentOutOfRangeException(nameof(offsetCrop), "Cropping rectangle is out of range.");
                }

                using (Bitmap clone = bitmap.Clone(
                    elementRect,
                    bitmap.PixelFormat))
                {
                    clone.Save(filePath, format);
                }
            }
        }

        private ImageFormat GetImageFormat(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).Replace(".", string.Empty);
            return IWebDriverExtensions.KnownImageFormats[fileExtension.ToLower()];
        }

        private Rectangle GetCurrentElementRectangle()
        {
            var currentPage = this.GetPageFromContext();

            var nativePage = currentPage.GetNativePage();

            WebElement element = nativePage as WebElement;

            return new Rectangle(
                element.LocationOnScreenOnceScrolledIntoView,
                element.Size);
        }

        private Rectangle GetElementRectangle(string fieldName)
        {
            var currentPage = this.GetPageFromContext();

            IPropertyData propertyData;
            bool elementExists = currentPage.TryGetElement(fieldName.ToLookupKey(), out propertyData);
            Assert.IsTrue(elementExists, $"Property '{fieldName.ToLookupKey()}' does not exist on page {currentPage.PageType.Name}");

            var nativePage = currentPage.GetNativePage();

            PropertyInfo propertyInfo = currentPage.PageType.GetProperty(propertyData.Name);

            WebElement element = propertyInfo.GetValue(nativePage, null) as WebElement;
            Assert.IsNotNull(element, $"Element '{fieldName.ToLookupKey()}' on page '{currentPage.PageType.Name}' is not a WebElement");

            return new Rectangle(
                element.LocationOnScreenOnceScrolledIntoView,
                element.Size);
        }

        /// <summary>
        /// Gets the page from the scenario context.
        /// </summary>
        /// <returns>The current page.</returns>
        private IPage GetPageFromContext()
        {
            var page = this.scenarioContext.GetCurrentPage();
            if (page == null)
            {
                throw new PageNavigationException("No page has been set as being the current page.");
            }

            return page;
        }
    }
}
