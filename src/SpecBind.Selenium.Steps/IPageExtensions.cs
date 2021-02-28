// <copyright file="IPageExtensions.cs" company="SpecBind">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Selenium.Steps
{
    using System.Reflection;
    using SpecBind.Pages;

    /// <summary>
    /// IPage Extensions.
    /// </summary>
    public static class IPageExtensions
    {
        /// <summary>
        /// Gets the native page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The native page.</returns>
        public static object GetNativePage(this IPage page)
        {
            MethodInfo methodInfo = typeof(IPage).GetMethod("GetNativePage");
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(page.PageType);
            return genericMethodInfo.Invoke(page, null);
        }
    }
}
