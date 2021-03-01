// <copyright file="MaximizeWindowAction.cs" company="">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Actions
{
    using SpecBind.ActionPipeline;
    using SpecBind.BrowserSupport;

    /// <summary>
    /// Maximize Window Action
    /// </summary>
    public class MaximizeWindowAction : ActionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximizeWindowAction" /> class.
        /// </summary>
        public MaximizeWindowAction()
            : base(typeof(MaximizeWindowAction).Name)
        {
        }

        /// <summary>
        /// Executes this instance action.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>
        /// The result of the action.
        /// </returns>
        public override ActionResult Execute(ActionContext actionContext)
        {
            IBrowser browser = WebDriverSupport.CurrentBrowser;

            browser.Maximize();

            return ActionResult.Successful();
        }
    }
}
