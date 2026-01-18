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

        // Nota: O Entity Framework Core mapeia automaticamente TimeSpan para o tipo 'time' no SQL Server
        // Não é necessário configuração adicional no OnModelCreating para os campos TimeSpan
        // nos models Escala, ModeloEscala e Operador

        public DbSet<Operador> Operadores { get; set; }
        public DbSet<Escala> Escalas { get; set; }
        public DbSet<ModeloEscala> ModelosEscala { get; set; }
    }
}