using Microsoft.AspNetCore.Identity;

namespace EvidenciaPoistencov.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? PoistenecId { get; set; }

        public Poistenec? Poistenec { get; set; }
    }
}