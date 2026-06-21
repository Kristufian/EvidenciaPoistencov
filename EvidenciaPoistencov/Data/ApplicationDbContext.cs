using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EvidenciaPoistencov.Models;

namespace EvidenciaPoistencov.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Poistenec> Poistenci { get; set; }

        public DbSet<Poistenie> Poistenia { get; set; }
    }
}
