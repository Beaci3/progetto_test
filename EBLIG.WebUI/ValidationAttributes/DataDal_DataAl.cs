using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class DataDal_DataAl : ValidationAttribute, IClientValidatable
    {

        public string DataAlField { get; set; }

        public object DataAlRequired { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            DateTime? getDate(object val)
            {
                try
                {
                    DateTime.TryParse(val.ToString(), out DateTime _d);
                    return _d;
                }
                catch (Exception)
                {
                    return null;
                }
            };

            var type = validationContext.ObjectInstance.GetType();

            var _other = type.GetProperty(DataAlField).GetValue(validationContext.ObjectInstance);

            if (value != null && _other == null)
            {
                return ValidationResult.Success;
            }

            if (getDate(_other).GetValueOrDefault() > getDate(value).GetValueOrDefault())
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
                ValidationType = "datadaldataal"
            };
            mvr.ValidationParameters.Add("dataalfield", DataAlField);
            mvr.ValidationParameters.Add("dataalrequired", DataAlRequired);
            return new[] { mvr };
        }
    }
}