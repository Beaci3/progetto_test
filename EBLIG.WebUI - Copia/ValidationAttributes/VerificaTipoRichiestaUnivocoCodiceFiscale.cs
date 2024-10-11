using EBLIG.DOM;
using EBLIG.DOM.DAL;
using EBLIG.WebUI.Controllers;
using EBLIG.WebUI.Helpers;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class VerificaTipoRichiestaUnivocoCodiceFiscaleValidator : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// se unica true, non si fa controllo su aziendaid
        /// </summary>
        public bool Unica { get; set; } = false;

        public string NomeCampo { get; set; } = "CodiceFiscale";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }

                int getInt(object v)
                {
                    try
                    {
                        if (v==null)
                        {
                            return 0;
                        }

                        return (int)v;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                };

                var type = validationContext.ObjectInstance.GetType();
                var _AziendaId = getInt(type.GetProperty("AziendaId").GetValue(validationContext.ObjectInstance));
                var _TipoRichiestaId = getInt(type.GetProperty("TipoRichiestaId").GetValue(validationContext.ObjectInstance));
                var _RichiestaId = getInt(type.GetProperty("RichiestaId").GetValue(validationContext.ObjectInstance));

                if (PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(_AziendaId, _TipoRichiestaId, value?.ToString(), _RichiestaId, NomeCampo, Unica))
                {
                    return new ValidationResult(ErrorMessage);
                }

                return ValidationResult.Success;

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
                ValidationType = "verificatiporichiestaunivococodicefiscalevalidator"
            };

            return new[] { mvr };
        }
    }
}