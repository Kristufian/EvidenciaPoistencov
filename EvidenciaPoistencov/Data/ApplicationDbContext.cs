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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasOne(u => u.Poistenec).WithOne().HasForeignKey<ApplicationUser>(u => u.PoistenecId).OnDelete(DeleteBehavior.Restrict);
        }
    }

}
