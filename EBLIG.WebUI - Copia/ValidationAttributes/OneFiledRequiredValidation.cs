using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class OneFiledRequiredValidation : ValidationAttribute, IClientValidatable
    {
        public string Other { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var type = validationContext.ObjectInstance.GetType();

            var _other = (object)type.GetProperty(Other).GetValue(validationContext.ObjectInstance);

            if (string.IsNullOrWhiteSpace(value?.ToString()) || string.IsNullOrWhiteSpace(_other?.ToString()))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "onefiledrequiredvalidation"
            };

            mvr.ValidationParameters.Add("other", Other);
            return new[] { mvr };
        }
    }
}