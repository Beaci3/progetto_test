using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EBLIG.WebUI.Models
{
    public class IdentityUserEBLIG : IdentityUser
    {
        /// <summary>
        /// Dopo il primo accesso, sarà obbligatorio 
        /// compilare una serie di informazioni personali.
        /// </summary>
        public bool InformazioniPersonaliCompilati { get; set; }
        
        public string DocumentoIdentita { get; set; }

        public string Cognome { get; set; }

        public string Nome { get; set; }
    }

    // È possibile aggiungere dati del profilo per l'utente aggiungendo altre proprietà alla classe ApplicationUser. Per altre informazioni, vedere https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUserEBLIG
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenere presente che il valore di authenticationType deve corrispondere a quello definito in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Aggiungere qui i reclami utente personalizzati
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("EBLIGDbContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}