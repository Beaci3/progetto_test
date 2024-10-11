using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class DocumentiObblicatori : ValidationAttribute, IClientValidatable
    {
        public string AllegatiIdSelInput { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var type = validationContext.ObjectInstance.GetType();

            if (value != null)
            {
                var _allegatiidselinput = type.GetProperty(AllegatiIdSelInput).GetValue(validationContext.ObjectInstance);

                if (_allegatiidselinput == null)
                {
                    return new ValidationResult(ErrorMessage);
                }
                var _allegatiidselinputArray= _allegatiidselinput.ToString().Split(',');

                var _allegatiidArray = value.ToString().Split(',');

                var _c = _allegatiidArray.Except(_allegatiidselinputArray);

                return _c.Count() > 0 ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "documentiobblicatori"
            };

            mvr.ValidationParameters.Add("allegatiidselinput", AllegatiIdSelInput);
            return new[] { mvr };
        }
    }
}