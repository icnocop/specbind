// <copyright file="PageHistoryException.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.ActionPipeline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Pages;

    /// <summary>
    /// Page History Exception.
    /// </summary>
    /// <seealso cref="System.ApplicationException" />
    public class PageHistoryException : ApplicationException
    {
        private readonly string propertyName;
        private readonly Dictionary<Type, IPage> pageHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHistoryException" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="pageHistory">The page history.</param>
        public PageHistoryException(string propertyName, Dictionary<Type, IPage> pageHistory)
        {
            this.propertyName = propertyName;
            this.pageHistory = pageHistory;
        }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                string message = $"A property with the name '{this.propertyName}' was not found in any of the displayed pages:";

                if (this.pageHistory != null)
                {
                    message += Environment.NewLine;
                    message += string.Join(Environment.NewLine, this.pageHistory.Select(x => x.Key.Name));
                }

                return message;
            }
        }
    }
}
