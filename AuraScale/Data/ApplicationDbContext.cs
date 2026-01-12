using AuraScale.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuraScale.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Operador> Operadores { get; set; }
        public DbSet<Escala> Escalas { get; set; }
        public DbSet<ModeloEscala> ModelosEscala { get; set; }
    }
}