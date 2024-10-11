using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class ChecksumCFPiva : ValidationAttribute, IClientValidatable
    {
        public bool RequiredPivaOrCF { get; set; }

        public bool Required { get; set; } = true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (!Required)
                {
                    if (string.IsNullOrEmpty(value?.ToString()))
                    {
                        return ValidationResult.Success;
                    }
                }

                if (value == null)
                {
                    throw new Exception("");
                }

                string cf = value.ToString().ToUpper();

                if (RequiredPivaOrCF)
                {
                    if (cf.Length == 11)
                    {
                        if (Regex.IsMatch(cf, "[0-9]{11}"))
                        {
                            return ValidationResult.Success;
                        }
                    }
                }

                if (cf.Length != 16)
                {
                    throw new Exception("");
                }

                var validi = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                for (var i = 0; i < 16; i++)
                {
                    if (validi.IndexOf(cf[i]) == -1)
                        throw new Exception("");
                }

                var set1 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var set2 = "ABCDEFGHIJABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var setpari = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var setdisp = "BAKPLCQDREVOSFTGUHMINJWZYX";
                var s = 0;

                for (var i = 1; i <= 13; i += 2)
                    s += setpari.IndexOf(set2[set1.IndexOf(cf[i])]);

                for (var i = 0; i <= 14; i += 2)
                    s += setdisp.IndexOf(set2[set1.IndexOf(cf[i])]);

                if (s % 26 != ((int)cf[15]) - ((int)'A'))
                    throw new Exception("");

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "checksumcfpiva"
            };
            mvr.ValidationParameters.Add("requiredpivaorcf", RequiredPivaOrCF);
            mvr.ValidationParameters.Add("required", Required);

            return new[] { mvr };
        }
    }
}