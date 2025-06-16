using System;
using Microsoft.EntityFrameworkCore;
using PujaService.Dominio.Entidades;
using PujaService.Infraestructura.Entidades;
using PujaService.Infraestructura.Persistencia;

namespace PujaService.Tests.Seeds
{
    public static class DataSeed
    {
        public static Guid PujaId = Guid.NewGuid();
        public static Guid SubastaId = Guid.NewGuid();
        public static Guid UsuarioId = Guid.NewGuid();
        public static Guid OtraPujaId = Guid.NewGuid();

        public static void Seed(PujaDbContext context)
        {
            // Limpiar pujas existentes
            context.Pujas.RemoveRange(context.Pujas);
            context.SaveChanges();

            // Agregar una puja
            var puja = new PujaEntity
            {
                Id = DataSeed.PujaId,
                SubastaId = DataSeed.SubastaId.ToString(),
                UsuarioId = DataSeed.UsuarioId.ToString(),
                Monto = 150,
                FechaHora = DateTime.UtcNow
            };


            context.Pujas.Add(puja);
            context.SaveChanges();
        }

        public static PujaDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<PujaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new PujaDbContext(options);
            Seed(context);
            return context;
        }
    }
}