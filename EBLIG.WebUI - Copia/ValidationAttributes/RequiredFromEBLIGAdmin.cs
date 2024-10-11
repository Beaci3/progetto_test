using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class RequiredFromEBLIGAdmin : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = HttpContext.Current.User;

            if (!user.IsInRole(IdentityHelper.Roles.Admin.ToString()))
            {
                return ValidationResult.Success;
            }

            if (value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "requiredfromebligadmin"
            };
            return new[] { mvr };
        }
    }
}