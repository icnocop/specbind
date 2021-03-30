// <copyright file="ElementLocatorDelegates.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.MoqHelpers
{
    using SpecBind.Pages;

    /// <summary>
    /// Element Locator Delgates
    /// </summary>
    public static class ElementLocatorDelegates
    {
        /// <summary>
        /// A delegate for the TryGetProperty method.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="output">The output.</param>
        public delegate void TryGetPropertyMethodDelegate(string propertyName, out IPropertyData output);
    }
}
