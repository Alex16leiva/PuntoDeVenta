﻿using CrossCutting.Validator;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CrossCutting.Network.Validator
{
    public class DataAnnotationsEntityValidator
                : IEntityValidator
    {
        #region Private Methods

        /// <summary>
        /// Get erros if object implement IValidatableObject
        /// </summary>
        /// <typeparam name="TEntity">The typeof entity</typeparam>
        /// <param name="item">The item to validate</param>
        /// <param name="errors">A collection of current errors</param>
        void SetValidatableObjectErrors<TEntity>(TEntity item, List<string> errors) where TEntity : class
        {
            if (typeof(IValidatableObject).IsAssignableFrom(typeof(TEntity)))
            {
                var validationContext = new ValidationContext(item, null, null);

                var validationResults = ((IValidatableObject)item).Validate(validationContext);

                errors.AddRange(validationResults.Select(vr => vr.ErrorMessage));
            }
        }

        void SetValidationAttributeErrors<TEntity>(TEntity item, List<string> errors) where TEntity : class
        {
            var result = from property in TypeDescriptor.GetProperties(item).Cast<PropertyDescriptor>()
                         from attribute in property.Attributes.OfType<ValidationAttribute>()
                         where !attribute.IsValid(property.GetValue(item))
                         select attribute.FormatErrorMessage(string.Empty);

            if (result != null && result.Any())
            {
                errors.AddRange(result);
            }
        }

        #endregion

        #region IEntityValidator Members

        public bool IsValid<TEntity>(TEntity item) where TEntity : class
        {

            if (item == null)
                return false;

            var validationErrors = new List<string>();

            SetValidatableObjectErrors(item, validationErrors);
            SetValidationAttributeErrors(item, validationErrors);

            return !validationErrors.Any();
        }

        public IEnumerable<string> GetInvalidMessages<TEntity>(TEntity item) where TEntity : class
        {
            if (item == null)
                return null;

            var validationErrors = new List<string>();

            SetValidatableObjectErrors(item, validationErrors);
            SetValidationAttributeErrors(item, validationErrors);
            return validationErrors;
        }

        #endregion
    }
}
