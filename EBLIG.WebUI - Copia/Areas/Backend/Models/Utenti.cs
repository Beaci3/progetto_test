using EBLIG.DOM.Entitys;
using EBLIG.WebUI.Models;
using EBLIG.WebUI.ValidationAttributes;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace EBLIG.WebUI.Areas.Backend.Models
{
    public class UtentiModificaPasswordViewModel
    {
        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        [DisplayName("Password vecchia")]
        public string PasswordVecchia { get; set; }

        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        [DisplayName("Password nuova")]
        public string PasswordNuova { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("PasswordNuova")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [DisplayName("Conferma Password nuova")]
        public string PasswordNuovaRipedi { get; set; }
    }

    public class UtentiRicercaModel
    {
        public string UtentiRicercaModel_Username { get; set; }
       
        public string UtentiRicercaModel_RuoId { get; set; }

        public string UtentiRicercaModel_Bloccato { get; set; }

        public string UtentiRicercaModel_EmailConfermata { get; set; }

        public List<(string, string, string)> Ruoli { get; internal set; }
    }

    public class UtentiRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<UtentiViewModel> Result { get; set; }

        public UtentiRicercaModel Filtri { get; set; }
    }

    public class UtentiViewModel
    {
        public bool? ReadOnly { get; set; }

        public string UserId { get; set; }

        [MaxLength(25)]
        [MinLength(4, ErrorMessage = "Il campo Username deve essere almeno di 4 caratteri.")]
        [ChecksumCFPiva(ErrorMessage = "CF / P.Iva non è valido", Required = false, RequiredPivaOrCF = true)]
        [Required]
        public string UserName { get; set; }

        //[MaxLength(16)]
        //public string UserNameSportello { get; set; }

        [MaxLength(175)]
        [Required]
        public string Nome { get; set; }

        [MaxLength(175)]
        [Required]
        public string Cognome { get; set; }

        [Required]
        public string RuoloId { get; set; }

        public string Ruolo { get; set; }

        public string RuoloFriendlyName { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool? Bloccato { get; set; }

        public bool? EmailConfermata { get; set; }


        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [PasswordStrong(RequiredLength = 8, RequireDigit = true, RequireLowercase = true, RequireUppercase = true, RequireNonLetterOrDigit = true)]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}