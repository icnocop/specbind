// <copyright file="PageHistoryServiceFixture.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using static SpecBind.Tests.MoqHelpers.ElementLocatorDelegates;

    /// <summary>
    /// Page History Service Tests
    /// </summary>
    [TestClass]
    public class PageHistoryServiceFixture
    {
        private readonly Mock<ILogger> logger = new Mock<ILogger>();

        /// <summary>
        /// Verifies the ability to add a page to the page history service.
        /// </summary>
        [TestMethod]
        public void Add()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            // Act
            pageHistoryService.Add(page.Object);

            // Assert
            Assert.AreEqual(1, pageHistoryService.PageHistory.Count);
            IPage actualPage = pageHistoryService[typeof(FirstPage)];
            Assert.AreEqual(page.Object, actualPage);

            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the ability to check if a page is contained in the page history service.
        /// </summary>
        [TestMethod]
        public void Contains()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            // Act and Assert
            Assert.IsFalse(pageHistoryService.Contains(page.Object));

            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the ability to remove a page from the page history service.
        /// </summary>
        [TestMethod]
        public void Remove()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            pageHistoryService.Add(page.Object);

            // Act
            pageHistoryService.Remove(page.Object);

            // Assert
            Assert.AreEqual(0, pageHistoryService.PageHistory.Count);

            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the ability to get the current page from the page history service.
        /// </summary>
        [TestMethod]
        public void GetCurrentPage()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            pageHistoryService.Add(page.Object);

            // Act
            IPage actualPage = pageHistoryService.GetCurrentPage();

            // Assert
            Assert.AreEqual(page.Object, actualPage);

            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the ability to find a page containing a specified property from the page history service.
        /// </summary>
        [TestMethod]
        public void FindPageContainingProperty()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>();
            mockPropertyData.Setup(x => x.PropertyType).Returns(typeof(FirstPage));

            IPropertyData propertyData;
            page.Setup(x => x.TryGetProperty("firstpage", out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            pageHistoryService.Add(page.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPageContainingProperty("firstpage");

            // Assert
            Assert.AreEqual(page.Object, actualPage);

            mockPropertyData.VerifyAll();
            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the ability to find a page in the page history service.
        /// </summary>
        [TestMethod]
        public void FindPage()
        {
            // Arrange
            Mock<IPage> page = new Mock<IPage>();
            page.Setup(x => x.PageType).Returns(typeof(FirstPage));

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>();
            mockPropertyData.Setup(x => x.PropertyType).Returns(typeof(FirstPage));
            mockPropertyData.Setup(x => x.GetItemAsPage()).Returns(page.Object);

            IPropertyData propertyData;
            page.Setup(x => x.TryGetProperty("firstpage", out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            pageHistoryService.Add(page.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPage("firstpage");

            // Assert
            Assert.AreEqual(page.Object, actualPage);

            mockPropertyData.VerifyAll();
            page.VerifyAll();
        }

        /// <summary>
        /// Verifies the page history service can find the first page when there are two pages in the collection.
        /// </summary>
        [TestMethod]
        public void FindPage_WithTwoPagesAndFindingFirstPage_FirstPageIsFound()
        {
            // Arrange
            Mock<IPage> firstPage = new Mock<IPage>();
            firstPage.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);
            pageHistoryService.Add(firstPage.Object);

            Mock<IPage> secondPage = new Mock<IPage>();
            secondPage.Setup(x => x.PageType).Returns(typeof(SecondPage));

            pageHistoryService.Add(secondPage.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPage("firstpage");

            // Assert
            Assert.AreEqual(firstPage.Object, actualPage);

            secondPage.VerifyAll();
            firstPage.VerifyAll();
        }

        /// <summary>
        /// Verifies the page history service can find the second page when there are two pages in the collection.
        /// </summary>
        [TestMethod]
        public void FindPage_WithTwoPagesAndFindingSecondPage_SecondPageIsFound()
        {
            // Arrange
            Mock<IPage> firstPage = new Mock<IPage>();
            firstPage.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);
            pageHistoryService.Add(firstPage.Object);

            Mock<IPage> secondPage = new Mock<IPage>();
            secondPage.Setup(x => x.PageType).Returns(typeof(SecondPage));

            pageHistoryService.Add(secondPage.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPage("secondpage");

            // Assert
            Assert.AreEqual(secondPage.Object, actualPage);

            secondPage.VerifyAll();
            firstPage.VerifyAll();
        }

        /// <summary>
        /// Verifies the page history service can find the first page when searching by its property name and there are two pages in the collection.
        /// </summary>
        [TestMethod]
        public void FindPage_WithTwoPagesAndPropertyNameInFirstPage_FirstPageIsFound()
        {
            // Arrange
            Mock<IPage> firstPage = new Mock<IPage>();
            firstPage.Setup(x => x.PageType).Returns(typeof(FirstPage));

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>();
            mockPropertyData.Setup(x => x.PropertyType).Returns(typeof(FirstPage));
            mockPropertyData.Setup(x => x.GetItemAsPage()).Returns(firstPage.Object);

            IPropertyData propertyData;
            firstPage.Setup(x => x.TryGetProperty("myfirstpage", out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);
            pageHistoryService.Add(firstPage.Object);

            Mock<IPage> secondPage = new Mock<IPage>();
            secondPage.Setup(x => x.PageType).Returns(typeof(SecondPage));

            pageHistoryService.Add(secondPage.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPage("myfirstpage");

            // Assert
            Assert.AreEqual(firstPage.Object, actualPage);

            secondPage.VerifyAll();
            mockPropertyData.VerifyAll();
            firstPage.VerifyAll();
        }

        /// <summary>
        /// Verifies the page history service can find the second page when searching by its property name and there are two pages in the collection.
        /// </summary>
        [TestMethod]
        public void FindPage_WithTwoPagesAndPropertyNameInSecondPage_SecondPageIsFound()
        {
            // Arrange
            Mock<IPage> firstPage = new Mock<IPage>();
            firstPage.Setup(x => x.PageType).Returns(typeof(FirstPage));

            PageHistoryService pageHistoryService = new PageHistoryService(this.logger.Object);

            pageHistoryService.Add(firstPage.Object);

            Mock<IPage> secondPage = new Mock<IPage>();
            secondPage.Setup(x => x.PageType).Returns(typeof(SecondPage));

            Mock<IPropertyData> mockPropertyData = new Mock<IPropertyData>();
            mockPropertyData.Setup(x => x.PropertyType).Returns(typeof(SecondPage));
            mockPropertyData.Setup(x => x.GetItemAsPage()).Returns(secondPage.Object);

            IPropertyData propertyData;
            secondPage.Setup(x => x.TryGetProperty("mysecondpage", out propertyData))
                .Callback(new TryGetPropertyMethodDelegate((string s, out IPropertyData p) => { p = mockPropertyData.Object; }))
                .Returns(true);

            pageHistoryService.Add(secondPage.Object);

            // Act
            IPage actualPage = pageHistoryService.FindPage("mysecondpage");

            // Assert
            Assert.AreEqual(secondPage.Object, actualPage);

            secondPage.VerifyAll();
            mockPropertyData.VerifyAll();
            firstPage.VerifyAll();
        }

        private class FirstPage
        {
        }

        private class SecondPage
        {
        }
    }
}
