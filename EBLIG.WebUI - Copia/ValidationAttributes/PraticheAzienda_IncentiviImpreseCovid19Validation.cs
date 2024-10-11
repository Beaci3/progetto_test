using EBLIG.DOM.DAL;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class PraticheAzienda_IncentiviImpreseCovid19Validation : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }

                int.TryParse(value?.ToString(), out int giorni);

                var type = validationContext.ObjectInstance.GetType();

                var _getvalueImportoErogato = type.GetProperty("ImportoTotaleRimborsato").GetValue(validationContext.ObjectInstance);

                decimal.TryParse(_getvalueImportoErogato?.ToString(), out decimal importoerogato);

                var _imortoCalcolato = PraticheAziendaUtility.GetImportoErogatoIncentiviCovid19Imprese(giorni);

                if (_imortoCalcolato == importoerogato)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage);

            }
            catch (System.Exception ex)
            {
                return new ValidationResult(ex.Message);

            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "praticheaziendaincentiviimpresecovid"
            };

            mvr.ValidationParameters.Add("importoerogatoelement", "ImportoErogato");

            return new[] { mvr };
        }
    }
}