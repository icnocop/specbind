// <copyright file="IPageHistoryService.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Helpers
{
    using System;
    using System.Collections.Generic;
    using SpecBind.Pages;

    /// <summary>
    /// An interface that keeps a history of pages that haven't been explicitly indicated as being closed.
    /// </summary>
    public interface IPageHistoryService
    {
        /// <summary>
        /// Gets or sets the page history.
        /// </summary>
        /// <value>The page history.</value>
        Dictionary<Type, IPage> PageHistory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPage"/> with the specified page type.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns>The page.</returns>
        IPage this[Type pageType] { get; }

        /// <summary>
        /// Adds the specified page to the history.
        /// </summary>
        /// <param name="page">The page.</param>
        void Add(IPage page);

        /// <summary>
        /// Determines whether the specified page is in the history.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns><c>true</c> if the specified page is in the history; otherwise, <c>false</c>.</returns>
        bool Contains(IPage page);

        /// <summary>
        /// Determines whether the specified page is in the history.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <returns><c>true</c> if the specified page is in the history; otherwise, <c>false</c>.</returns>
        bool Contains(Type pageType);

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <returns>The page.</returns>
        IPage GetCurrentPage();

        /// <summary>
        /// Finds the page containing the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The page.</returns>
        IPage FindPageContainingProperty(string propertyName);

        /// <summary>
        /// Finds the page with the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The page.</returns>
        IPage FindPage(string propertyName);

        /// <summary>
        /// Removes the specified page from the history.
        /// </summary>
        /// <param name="page">The page.</param>
        void Remove(IPage page);
    }
}