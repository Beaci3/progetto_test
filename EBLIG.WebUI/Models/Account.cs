using EBLIG.WebUI.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace EBLIG.WebUI.Models
{
    public class LoginViewModel
    {
        [MaxLength(35)]
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [MaxLength(25)]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[Display(Name = "Memorizza account")]
        //public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [MaxLength(75)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
  
    public class ResetPasswordViewModel
    {
        [MaxLength(35)]
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

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

        public string Code { get; set; }
    }
}