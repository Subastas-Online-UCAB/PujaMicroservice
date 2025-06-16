using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PujaService.Dominio.Entidades;
using PujaService.Infraestructura.Persistencia;
using PujaService.Infraestructura.Repositorios;
using Xunit;

namespace PujaService.test.Infraestructura.Repositorios
{
    public class PujaPostgresRepositoryTests
    {
        private PujaDbContext CrearContextoEnMemoria()
        {
            var options = new DbContextOptionsBuilder<PujaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new PujaDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GuardarPujaAsync_DeberiaGuardarCorrectamente()
        {
            var context = CrearContextoEnMemoria();
            var repository = new PujaPostgresRepository(context);

            var puja = new Puja("subasta123", "usuario123", 500m, false);

            await repository.GuardarPujaAsync(puja);
            var guardada = await context.Pujas.FirstOrDefaultAsync();

            Assert.NotNull(guardada);
            Assert.Equal(puja.Id, guardada.Id);
            Assert.Equal("subasta123", guardada.SubastaId);
            Assert.Equal("usuario123", guardada.UsuarioId);
            Assert.Equal(500m, guardada.Monto);
        }

        [Fact]
        public async Task ObtenerUltimaPujaAsync_DeberiaRetornarLaMasReciente()
        {
            var context = CrearContextoEnMemoria();
            var repository = new PujaPostgresRepository(context);

            var antigua = new Puja("s1", "u1", 100m, false) { FechaHora = DateTime.UtcNow.AddMinutes(-10) };
            var reciente = new Puja("s1", "u2", 300m, false) { FechaHora = DateTime.UtcNow };

            await repository.GuardarPujaAsync(antigua);
            await repository.GuardarPujaAsync(reciente);

            var ultima = await repository.ObtenerUltimaPujaAsync("s1");

            Assert.NotNull(ultima);
            Assert.Equal(reciente.Id, ultima.Id);
        }

        [Fact]
        public async Task ObetenerPujasSubastaAsync_DeberiaRetornarOrdenadasPorFecha()
        {
            var context = CrearContextoEnMemoria();
            var repository = new PujaPostgresRepository(context);

            var p1 = new Puja("s1", "u1", 100, false) { FechaHora = DateTime.UtcNow.AddMinutes(-15) };
            var p2 = new Puja("s1", "u2", 200, false) { FechaHora = DateTime.UtcNow.AddMinutes(-5) };
            var p3 = new Puja("s1", "u3", 300, false) { FechaHora = DateTime.UtcNow };

            await repository.GuardarPujaAsync(p1);
            await repository.GuardarPujaAsync(p2);
            await repository.GuardarPujaAsync(p3);

            var pujas = await repository.ObetenerPujasSubastaAsync("s1");

            Assert.Equal(3, pujas.Count);
            Assert.Equal(p3.Id, pujas[0].Id);
            Assert.Equal(p2.Id, pujas[1].Id);
            Assert.Equal(p1.Id, pujas[2].Id);
        }
    }
}
