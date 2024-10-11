using EBLIG.DOM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class PraticheAzienda_EventiEccezionaliCalamitaNaturaliImportoRiconosciutoValidation : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                decimal? getDecimal(object val)
                {
                    try
                    {
                        decimal.TryParse(val?.ToString(), out decimal v);

                        return v;
                    }
                    catch
                    {
                        return null;
                    }
                };

                var type = validationContext.ObjectInstance.GetType();
                var _TotaleDanniStruttureAttrezzatureElement = getDecimal(type.GetProperty("TotaleDanniStruttureAttrezzature").GetValue(validationContext.ObjectInstance));
                var _TotaleDanniScorteElement = getDecimal(type.GetProperty("TotaleDanniScorte").GetValue(validationContext.ObjectInstance));
                var _ImportoRiconosciuto = getDecimal(value);

                var _importoRiconoscito= PraticheAziendaUtility.GetImportoEventiEccezionaliCalamitaNaturaliImprese(_TotaleDanniStruttureAttrezzatureElement, _TotaleDanniScorteElement);

                if (_importoRiconoscito == _ImportoRiconosciuto)
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
                ValidationType = "praticheaziendaeventieccezionalicalamitanaturaliimportoriconosciuto"
            };

            return new[] { mvr };
        }
    }
}