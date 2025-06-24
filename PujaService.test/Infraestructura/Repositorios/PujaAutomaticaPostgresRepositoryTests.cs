using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PujaService.Dominio.Entidades;
using PujaService.Infraestructura.Persistencia;
using PujaService.Infraestructura.Repositorios;
using Xunit;

namespace PujaService.test.Infraestructura.Repositorios
{
    public class PujaAutomaticaPostgresRepositoryTests
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
        public async Task GuardarConfiguracionAsync_DeberiaCrearNuevaSiNoExiste()
        {
            var context = CrearContextoEnMemoria();
            var repo = new PujaAutomaticaPostgresRepository(context);

            var config = new configuracionPujaAutomatica("subasta1", "usuario1", 500, 10);

            await repo.GuardarConfiguracionAsync(config);

            var guardada = await context.PujasAutomaticas.FirstOrDefaultAsync();
            Assert.NotNull(guardada);
            Assert.Equal("subasta1", guardada.SubastaId);
            Assert.Equal("usuario1", guardada.UsuarioId);
        }

        


        [Fact]
        public async Task ObtenerConfiguracionesPorSubastaAsync_DeberiaRetornarCoincidentes()
        {
            var context = CrearContextoEnMemoria();
            var repo = new PujaAutomaticaPostgresRepository(context);

            context.PujasAutomaticas.AddRange(
                new configuracionPujaAutomatica("s1", "u1", 300, 10),
                new configuracionPujaAutomatica("s1", "u2", 400, 20),
                new configuracionPujaAutomatica("s2", "u3", 500, 30)
            );
            await context.SaveChangesAsync();

            var result = await repo.ObtenerConfiguracionesPorSubastaAsync("s1");

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.Equal("s1", c.SubastaId));
        }

        [Fact]
        public async Task ObtenerPorUsuarioYSubastaAsync_DeberiaRetornarConfiguracionCorrecta()
        {
            var context = CrearContextoEnMemoria();
            var repo = new PujaAutomaticaPostgresRepository(context);

            var target = new configuracionPujaAutomatica("s9", "u9", 999, 9);
            context.PujasAutomaticas.Add(target);
            await context.SaveChangesAsync();

            var result = await repo.ObtenerPorUsuarioYSubastaAsync("u9", "s9");

            Assert.NotNull(result);
            Assert.Equal(target.Id, result!.Id);
        }
    }
}
