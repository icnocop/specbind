// <copyright file="IWebDriverExtensions.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using OpenQA.Selenium;
    using SpecBind.Selenium;

    /// <summary>
    /// IWebDriver Extensions.
    /// </summary>
    public static class IWebDriverExtensions
    {
        /// <summary>
        /// The known image formats
        /// </summary>
        public static readonly Dictionary<string, ImageFormat> KnownImageFormats = new Dictionary<string, ImageFormat>
        {
            { "bmp", ImageFormat.Bmp },
            { "gif", ImageFormat.Gif },
            { "ico", ImageFormat.Icon },
            { "jpg", ImageFormat.Jpeg },
            { "jpeg", ImageFormat.Jpeg },
            { "png", ImageFormat.Png },
            { "tif", ImageFormat.Tiff },
            { "tiff", ImageFormat.Tiff },
            { "wmf", ImageFormat.Wmf }
        };

        /// <summary>
        /// The known screenshot image formats
        /// </summary>
        public static readonly Dictionary<string, ScreenshotImageFormat> KnownScreenshotImageFormats = new Dictionary<string, ScreenshotImageFormat>
        {
            { "bmp", ScreenshotImageFormat.Bmp },
            { "gif", ScreenshotImageFormat.Gif },
            { "jpg", ScreenshotImageFormat.Jpeg },
            { "jpeg", ScreenshotImageFormat.Jpeg },
            { "png", ScreenshotImageFormat.Png },
            { "tif", ScreenshotImageFormat.Tiff },
            { "tiff", ScreenshotImageFormat.Tiff }
        };

        /// <summary>
        /// Gets the element image.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="element">The element.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="width">The width.</param>
        public static void TakeScreenshot(this IWebDriver webDriver, WebElement element, string filePath, int? width = null)
        {
            // get the bounding client rectangle because the element may extend off the page
            const string javascript = "return arguments[0].getBoundingClientRect()";
            var obj = (Dictionary<string, object>)((IJavaScriptExecutor)webDriver).ExecuteScript(javascript, element);
            Rectangle rect = new Rectangle(
                (int)double.Parse(obj["left"].ToString()),
                (int)double.Parse(obj["top"].ToString()),
                (int)double.Parse(obj["width"].ToString()),
                (int)double.Parse(obj["height"].ToString()));

            string fileExtension = Path.GetExtension(filePath).Replace(".", string.Empty);
            ImageFormat format = KnownImageFormats[fileExtension.ToLower()];

            using (Bitmap bitmap = PrintPage(webDriver))
            {
                rect.Intersect(new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                if (rect.IsEmpty)
                {
                    throw new ArgumentOutOfRangeException(nameof(element), "Cropping rectangle is out of range.");
                }

                if (width.HasValue)
                {
                    rect.Width = width.Value;
                }

                using (Bitmap clone = bitmap.Clone(
                    rect,
                    bitmap.PixelFormat))
                {
                    // crop white and gray space
                    using (Bitmap cropped = CropWhiteAndGraySpace(clone))
                    {
                        cropped.Save(filePath, format);
                    }
                }
            }
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="filePath">The file path.</param>
        public static void TakeScreenshot(this IWebDriver webDriver, string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).Replace(".", string.Empty);
            ImageFormat format = KnownImageFormats[fileExtension.ToLower()];
            ScreenshotImageFormat imageFormat = KnownScreenshotImageFormats[fileExtension.ToLower()];

            using (Bitmap bitmap = PrintPage(webDriver))
            {
                bitmap.Save(filePath, format);
            }
        }

        /// <summary>
        /// Takes a screenshot.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public static void TakeScreenshot(this IWebDriver webDriver, string filePath, int? width = null, int? height = null)
        {
            string fileExtension = Path.GetExtension(filePath).Replace(".", string.Empty);
            ImageFormat format = KnownImageFormats[fileExtension.ToLower()];

            Screenshot screenShot = (webDriver as ITakesScreenshot).GetScreenshot();
            using (MemoryStream memoryStream = new MemoryStream(screenShot.AsByteArray))
            {
                using (Bitmap bitmap = new Bitmap(memoryStream))
                {
                    int targetWidth = bitmap.Width;
                    if (width.HasValue)
                    {
                        targetWidth = width.Value;
                    }

                    int targetHeight = bitmap.Height;
                    if (height.HasValue)
                    {
                        targetHeight = height.Value;
                    }

                    Rectangle cropRect = new Rectangle(0, 0, targetWidth, targetHeight);

                    using (Bitmap target = new Bitmap(cropRect.Width, cropRect.Height))
                    {
                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(
                                bitmap,
                                new Rectangle(0, 0, target.Width, target.Height),
                                cropRect,
                                GraphicsUnit.Pixel);
                        }

                        target.Save(filePath, format);
                    }
                }
            }
        }

        private static Bitmap PrintPage(IWebDriver webDriver)
        {
            // Get the total size of the page
            var totalWidth = (int)webDriver.ExecuteScript<long>("return document.body.offsetWidth");
            var totalHeight = (int)webDriver.ExecuteScript<long>("return document.body.parentNode.scrollHeight");

            // Get the size of the viewport
            var viewportWidth = (int)webDriver.ExecuteScript<long>("return document.body.clientWidth");
            var viewportHeight = (int)webDriver.ExecuteScript<long>("return window.innerHeight");

            // We only care about taking multiple images together if it doesn't already fit
            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                var screenshot = webDriver.TakeScreenshot();
                return ScreenshotToBitmap(screenshot);
            }

            // Split the Screen in multiple Rectangles
            List<Rectangle> rectangles = new List<Rectangle>();

            // Loop until the Total Height is reached
            for (int y = 0; y < totalHeight; y += viewportHeight)
            {
                int newHeight = viewportHeight;

                // Fix if the Height of the Element is too big
                if (y + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - y;
                }

                // Loop until the Total Width is reached
                for (int x = 0; x < totalWidth; x += viewportWidth)
                {
                    int newWidth = viewportWidth;

                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - x;
                    }

                    // Create and add the Rectangle
                    Rectangle currRect = new Rectangle(x, y, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }

            // Build the Image
            var stitchedImage = new Bitmap(totalWidth, totalHeight);

            // Get all Screenshots and stitch them together
            Rectangle previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the Scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    int xDiff = rectangle.Right - previous.Right;
                    int yDiff = rectangle.Bottom - previous.Bottom;

                    // Scroll
                    webDriver.ExecuteScript(string.Format("window.scrollBy({0}, {1})", xDiff, yDiff));

                    // wait until the Chrome scrollbar disappears
                    System.Threading.Thread.Sleep(1000);
                }

                // Take Screenshot
                var screenshot = webDriver.TakeScreenshot();

                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);

                // Calculate the Source Rectangle
                Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                // Copy the Image
                using (Graphics g = Graphics.FromImage(stitchedImage))
                {
                    g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }

                // Set the Previous Rectangle
                previous = rectangle;
            }

            return stitchedImage;
        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }

            return screenshotImage;
        }

        private static Bitmap ScreenshotToBitmap(Screenshot screenshot)
        {
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                return new Bitmap(memStream);
            }
        }

        private static T ExecuteScript<T>(this IWebDriver webDriver, string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            return (T)js.ExecuteScript(script);
        }

        private static void ExecuteScript(this IWebDriver webDriver, string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            js.ExecuteScript(script);
        }

        private static Screenshot TakeScreenshot(this IWebDriver webDriver)
        {
            return (webDriver as ITakesScreenshot).GetScreenshot();
        }

        /// <summary>
        /// Crops the specified bitmap by removing white and gray spaces.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <returns>The bitmap without white and gray spaces</returns>
        private static Bitmap CropWhiteAndGraySpace(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                {
                    if (bmp.GetPixel(i, row).R < 229)
                    {
                        return false;
                    }
                }

                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                {
                    if (bmp.GetPixel(col, i).R < 229)
                    {
                        return false;
                    }
                }

                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                {
                    topmost = row;
                }
                else
                {
                    break;
                }
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                {
                    bottommost = row;
                }
                else
                {
                    break;
                }
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                {
                    leftmost = col;
                }
                else
                {
                    break;
                }
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                {
                    rightmost = col;
                }
                else
                {
                    break;
                }
            }

            if (rightmost == 0)
            {
                // As reached left
                rightmost = w;
            }

            if (bottommost == 0)
            {
                // As reached top.
                bottommost = h;
            }

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0)
            {
                // No border on left or right
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0)
            {
                // No border on top or bottom
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(
                        bmp,
                        new RectangleF(0, 0, croppedWidth, croppedHeight),
                        new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                        GraphicsUnit.Pixel);
                }

                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }
    }
}
