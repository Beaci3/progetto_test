using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using EBLIG.WebUI.Models;
using EBLIG.WebUI.Controllers;

namespace EBLIG.WebUI
{
    public class IdentityHelper
    {
        public enum Roles
        {
            Azienda,
            ConsulenteCs,
            Dipendente,
            Admin,
            Sportello
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            BaseController controller = new BaseController();
            controller.SendMailAsync(new SimpleMailMessage
            {
                ToName = message.Destination,
                ToEmail = message.Destination,
                Body = message.Body,
                Subject = message.Subject
            });

            // Inserire qui la parte di codice del servizio di posta elettronica per l'invio di un messaggio.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Inserire qui la parte di codice del servizio SMS per l'invio di un SMS.
            return Task.FromResult(0);
        }
    }

    // Configurare la gestione utenti dell'applicazione utilizzata in questa applicazione. UserManager viene definito in ASP.NET Identity ed è utilizzato dall'applicazione.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configurare la logica di convalida per i nomi utente
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configurare la logica di convalida per le password
            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 8,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true,
            //};

            // Configurare le impostazioni predefinite per il blocco dell'utente
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registrare i provider di autenticazione a due fattori. Questa applicazione usa il numero di telefono e gli indirizzi e-mail come metodi per ricevere un codice di verifica dell'utente
            // Si può scrivere un provider personalizzato e inserirlo qui.
            manager.RegisterTwoFactorProvider("Codice telefono", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Il codice di sicurezza è {0}"
            });
            manager.RegisterTwoFactorProvider("Codice e-mail", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Codice di sicurezza",
                BodyFormat = "Il codice di sicurezza è {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configurare la gestione accessi usata in questa applicazione.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // Configurare la gestione dei Ruoli in questa applicazione.
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store)
          : base(store)
        {
        }


        //Factory method
        public static ApplicationRoleManager Create(
          IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(
              new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}
