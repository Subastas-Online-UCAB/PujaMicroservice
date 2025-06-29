using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PujaService.Aplicacion.Handlers;
using PujaService.Aplicacion.Queries;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using Xunit;

namespace PujaService.Tests.Handlers
{
    public class GetPujasPorSubastaHandlerTests
    {
        [Fact]
        public async Task Handle_DeberiaRetornarListaDePujas()
        {
            // Arrange
            var subastaId = "abc123";
            var mockRepo = new Mock<IPujaMongoRepository>();
            mockRepo.Setup(repo => repo.ObtenerPorSubastaAsync(subastaId))
                .ReturnsAsync(new List<Puja>
                {
                    new Puja { Id = Guid.NewGuid(), SubastaId = subastaId, UsuarioId = "user1", Monto = 100, FechaHora = DateTime.UtcNow, EsAutomatica = false },
                    new Puja { Id = Guid.NewGuid(), SubastaId = subastaId, UsuarioId = "user2", Monto = 150, FechaHora = DateTime.UtcNow, EsAutomatica = true }
                });

            var handler = new GetPujasPorSubastaHandler(mockRepo.Object);
            var query = new GetPujasPorSubastaQuery(subastaId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].IdSubasta.Should().Be(subastaId);
        }
    }
}