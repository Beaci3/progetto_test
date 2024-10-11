using EBLIG.DOM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class PraticheAzienda_EventiEccezionaliCalamitaNaturaliDataEventoValidation : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }

                DateTime? getDate(object val)
                {
                    try
                    {
                        DateTime.TryParse(val?.ToString(), out DateTime v);

                        return v;
                    }
                    catch
                    {
                        return null;
                    }
                };

                var type = validationContext.ObjectInstance.GetType();
                var _MinDate = getDate(type.GetProperty("MinDate").GetValue(validationContext.ObjectInstance));
                var _MaxDate = getDate(type.GetProperty("MaxDate").GetValue(validationContext.ObjectInstance));
                var _dataEvento = getDate(value);

                if (_dataEvento >= _MinDate && _dataEvento <= _MaxDate)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage);

            }
            catch (Exception ex)
            {
                return new ValidationResult(ex.Message);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "praticheaziendaeventieccezionalicalamitanaturalidataevento"
            };

            //mvr.ValidationParameters.Add("mindateelement", MinDateElement);
            //mvr.ValidationParameters.Add("maxdateelement", MaxDateElement);

            return new[] { mvr };
        }
    }
}