using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class PasswordStrong : ValidationAttribute, IClientValidatable
    {
        public string RequiredPattern { get; set; }
        public int RequiredLength { get; set; } = 0;
        public bool RequireDigit { get; set; } = false;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireUppercase { get; set; } = false;
        public bool RequireNonLetterOrDigit { get; set; } = false;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string _value = value == null ? "" : value.ToString();
            var _hashValue = !string.IsNullOrEmpty(_value);

            var _RequiredLength = true;
            var _RequireNonLetterOrDigit = true;
            var _RequiredPattern = true;
            var _RequireLowercase = true;
            var _RequireUppercase = true;
            var _RequireDigit = true;

            var _text = "<li>Password e un campo obbligatorio</li>";
            var _ar = RequiredPattern?.ToCharArray();

            if (!string.IsNullOrEmpty(_value))
            {
                _text = "";

                if (RequiredLength > 0)
                {
                    //lunghezza minima password
                    _RequiredLength = value.ToString().Trim().Length >= RequiredLength;
                    if (!_RequiredLength)
                    {
                        _text += "<li>Password deve avere almeno " + RequiredLength + " caratteri</li>";
                    }
                }

                if (RequireNonLetterOrDigit)
                {
                    //verifica carattere speciale
                    _RequireNonLetterOrDigit = Regex.Replace(_value, "[A-Z0-9]", "", RegexOptions.IgnoreCase).Length > 0;
                    if (!_RequireNonLetterOrDigit)
                    {
                        _text += "<li>Password deve avere un carattere speciale</li>";
                    }
                }

                if (!string.IsNullOrWhiteSpace(RequiredPattern))
                {
                    //verifica pattern
                    foreach (var item in _ar)
                    {
                        if (_value.ToString().Contains(item.ToString()))
                        {
                            _RequiredPattern = true;
                            break;
                        }
                    }

                    if (!_RequiredPattern)
                    {
                        _text += "<li>Password deve contenere un carattere tra questi " + RequiredPattern + "</li>";
                    }
                }

                if (RequireLowercase)
                {
                    //verifica pattern
                    _RequireLowercase = Regex.IsMatch(_value, "[a-z]");
                    if (!_RequireLowercase)
                    {
                        _text += "<li>Password deve avere almeno una lettera minuscola</li>";
                    }
                }

                if (RequireUppercase)
                {
                    //verifica pattern
                    _RequireUppercase = Regex.IsMatch(_value, "[A-Z]");
                    if (!_RequireUppercase)
                    {
                        _text += "<li>Password deve avere almeno una lettera maiuscola</li>";
                    }
                }

                if (RequireDigit)
                {
                    //verifica numero
                    _RequireDigit = Regex.IsMatch(_value, "[0-9]");
                    if (!_RequireDigit)
                    {
                        _text += "<li>Password deve avere almeno un numero</li>";
                    }
                }
            }

            if (_hashValue && _RequiredLength && _RequireNonLetterOrDigit && _RequiredPattern && _RequireLowercase && _RequireUppercase && _RequireDigit)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("<ul>" + _text + "</ul>");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "passwordstrong"
            };

            mvr.ValidationParameters.Add("requiredpattern", RequiredPattern);
            mvr.ValidationParameters.Add("requiredlength", RequiredLength);
            mvr.ValidationParameters.Add("requiredigit", RequireDigit);
            mvr.ValidationParameters.Add("requirelowercase", RequireLowercase);
            mvr.ValidationParameters.Add("requireuppercase", RequireUppercase);
            mvr.ValidationParameters.Add("requirenonletterordigit", RequireNonLetterOrDigit);

            return new[] { mvr };
        }
    }
}