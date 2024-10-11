using EBLIG.WebUI.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace EBLIG.WebUI.Models
{
    public class RegistrazioneDipendente
    {
        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        public string ConfirmPassword { get; set; }

        [MaxLength(16)]
        [Required]
        [Display(Name = "Codice Fiscale")]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale è obbligatorio", RequiredPivaOrCF = false)]
        public string CodiceFiscale { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool PersonaFisica { get; set; }

        public string DocumentoIdentita { get; set; }
    }

    public class RegistrazioneAzienda
    {
        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome Titolare")]
        public string Nome { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome Titolare")]
        public string Cognome { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        public string ConfirmPassword { get; set; }

        [MaxLength(10)]
        [Required]
        [Display(Name = "Matricola Inps")]
        [RegularExpression("[0-9]{10}", ErrorMessage = "Il campo Matricola Inps non è valido")]
        public string MatricolaInps { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool PersonaFisica { get; set; }
    }

    public class RegistrazioneConsulenteCs
    {
        [MaxLength(175)]
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [MaxLength(175)]
        [Required]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        public string ConfirmPassword { get; set; }

        [MaxLength(16)]
        [Required]
        [ChecksumCFPiva(ErrorMessage = "Il campo Codice Fiscale o Partita Iva è obbligatorio", RequiredPivaOrCF = true)]
        [Display(Name = "Codice Fiscale o Partita Iva")]
        public string CodiceFiscalePIva { get; set; }

        [CheckBoxValidation(ErrorMessage = "Campo obbligatorio")]
        public bool PersonaFisica { get; set; }

    }

    public class RecuperoPasswordConfermaModel
    {
        public string UrlConferma { get; set; }
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class RegistrazioneConfermaModel
    {
        public string UrlConferma { get; set; }
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}