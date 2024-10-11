using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EBLIG.WebUI.ValidationAttributes
{
    public class IfIBAN : ValidationAttribute, IClientValidatable
    {
        public string Iban { get; set; }

        protected override ValidationResult IsValid(object value,
                          ValidationContext validationContext)
        {
            var _msg = validationContext != null ? validationContext.DisplayName : "IBAN non corretto.";

            return IsIbanChecksumValid(value)
                ? ValidationResult.Success
                : new ValidationResult(FormatErrorMessage(_msg));
        }

        static bool IsIbanChecksumValid(object _iban)
        {
            
            try
            {

                //Copyright (C) 2013 DvdKhl
                //Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
                //to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
                //and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
                //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
                //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
                //WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

                //Gabriel 16/11/2020
                //PUO ESSERE NULL
                if (string.IsNullOrWhiteSpace(_iban?.ToString()))
                {
                    return true;
                }

                var iban = _iban.ToString();

                if (iban.Length < 4 || iban[0] == ' ' || iban[1] == ' ' || iban[2] == ' ' || iban[3] == ' ')
                    throw new InvalidOperationException();

                var checksum = 0;
                var ibanLength = iban.Length;
                for (int charIndex = 0; charIndex < ibanLength; charIndex++)
                {
                    if (iban[charIndex] == ' ') continue;

                    int value;
                    var c = iban[(charIndex + 4) % ibanLength];
                    if ((c >= '0') && (c <= '9'))
                    {
                        value = c - '0';
                    }
                    else if ((c >= 'A') && (c <= 'Z'))
                    {
                        value = c - 'A';
                        checksum = (checksum * 10 + (value / 10 + 1)) % 97;
                        value %= 10;
                    }
                    else if ((c >= 'a') && (c <= 'z'))
                    {
                        value = c - 'a';
                        checksum = (checksum * 10 + (value / 10 + 1)) % 97;
                        value %= 10;
                    }
                    else return false;

                    checksum = (checksum * 10 + value) % 97;
                }
                return checksum == 1;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "ifiban"
            };

            mvr.ValidationParameters.Add("iban", Iban);
            return new[] { mvr };
        }
    }
}