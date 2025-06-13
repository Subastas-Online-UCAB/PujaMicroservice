using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PujaService.Infraestructura.Entidades;


namespace PujaService.Infraestructura.Persistencia
{
    public class PujaDbContext : DbContext
    {
        public PujaDbContext(DbContextOptions<PujaDbContext> options) : base(options) { }

        public DbSet<PujaEntity> Pujas => Set<PujaEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PujaEntity>(entity =>
            {
                entity.ToTable("Pujas");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Monto).HasColumnType("decimal(18,2)");
            });
        }
    }
}
