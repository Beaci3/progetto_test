using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class CustomRangeValidator : ValidationAttribute, IClientValidatable
    {
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string MinValueErrorMessage { get; set; }
        public string MaxValueErrorMessage { get; set; }
        public string ValidateOnlyForRoles { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = HttpContext.Current.User;

            if (!user.IsInRole(IdentityHelper.Roles.Admin.ToString()))
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                return new ValidationResult("Inserire un valore");
            }

            decimal? tryparse(object val)
            {
                try
                {
                    decimal.TryParse(val?.ToString(), out decimal v);

                    return v;
                }
                catch
                {
                    return 0;
                }
            };

            if (MinValue != null && tryparse(value) < tryparse(MinValue))
            {
                return new ValidationResult(MinValueErrorMessage);
            }

            if (MaxValue != null && tryparse(value) > tryparse(MaxValue))
            {
                return new ValidationResult(MaxValueErrorMessage);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var _validate = ValidateOnlyForRoles == null;

            if (ValidateOnlyForRoles != null)
            {
                _validate = false;

                foreach (var role in ValidateOnlyForRoles.Split(','))
                {
                    var user = HttpContext.Current.User;

                    if (user.IsInRole(role?.Trim()))
                    {
                        _validate = true;
                        break;
                    }

                }
            }


            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "customrangevalidator"
            };
            mvr.ValidationParameters.Add("validate", _validate);
            mvr.ValidationParameters.Add("minvalue", MinValue);
            mvr.ValidationParameters.Add("maxvalue", MaxValue);
            mvr.ValidationParameters.Add("minvalueerrormessage", MinValueErrorMessage);
            mvr.ValidationParameters.Add("maxvalueerrormessage", MaxValueErrorMessage);
            return new[] { mvr };


        }
    }
}