// <copyright file="ValidationTableExtensionMethods.cs">
//    Copyright © 2021 SpecBind. All rights reserved.
// </copyright>

namespace SpecBind.Tests.Extensions
{
    using System.Linq;
    using SpecBind.Validation;

    /// <summary>
    /// Validation Table Extension Methods
    /// </summary>
    public static class ValidationTableExtensionMethods
    {
        /// <summary>
        /// Validates the contents of the validation tables are equal.
        /// </summary>
        /// <param name="validationTable">The validation table.</param>
        /// <param name="otherValidationTable">The other validation table.</param>
        /// <returns>
        ///   <c>true</c> if the contents of the validation table are equal; otherwise <c>false</c>.
        /// </returns>
        public static bool ContentEquals(this ValidationTable validationTable, ValidationTable otherValidationTable)
        {
            return (otherValidationTable ?? new ValidationTable())
                .Validations.OrderBy(kvp => kvp.FieldName).Select(x => x.ToString()).ToArray()
                .SequenceEqual((validationTable ?? new ValidationTable())
                    .Validations.OrderBy(kvp => kvp.FieldName).Select(x => x.ToString()));
        }
    }
}
