using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Services.Helpers
{
    public static class ValidationHelper
    {
        public static void ModelValidation(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);

            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(
                    obj,
                    validationContext,
                    validationResults,
                    validateAllProperties: true
                );

            if (!isValid)
            {
                string? errorMessage = validationResults.FirstOrDefault()?.ErrorMessage; 
                throw new ArgumentException(errorMessage);
            }
        }

    }
}
