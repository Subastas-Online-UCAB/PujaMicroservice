using Xunit;
using Moq;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PujaService.Infraestructura.Repositorios;
using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;

namespace PujaService.Tests.Repositorios
{
    public class PujaMongoRepositoryTests
    {
        [Fact]
        public async Task ObtenerPorSubastaAsync_DeberiaRetornarListaDePujas()
        {
            // Arrange
            var subastaId = "sub123";
            var expectedPujas = new List<PujaMongoDto>
            {
                new PujaMongoDto
                {
                    Id = Guid.NewGuid(),
                    SubastaId = subastaId,
                    UsuarioId = "user1",
                    Monto = 100,
                    FechaHora = DateTime.UtcNow,
                    EsAutomatica = false
                },
                new PujaMongoDto
                {
                    Id = Guid.NewGuid(),
                    SubastaId = subastaId,
                    UsuarioId = "user2",
                    Monto = 200,
                    FechaHora = DateTime.UtcNow,
                    EsAutomatica = true
                }
            };

            // Cursor que simula resultados de Mongo
            var mockCursor = new Mock<IAsyncCursor<PujaMongoDto>>();
            mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                      .Returns(true)
                      .Returns(false);

            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);

            mockCursor.SetupGet(x => x.Current).Returns(expectedPujas);

            // Colección mock
            var collectionMock = new Mock<IMongoCollection<PujaMongoDto>>();

            collectionMock
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<PujaMongoDto>>(),
                    It.IsAny<FindOptions<PujaMongoDto, PujaMongoDto>>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(mockCursor.Object);

            // DB context mock
            var dbContextMock = new Mock<IMongoDbContext>();
            dbContextMock.Setup(db => db.Pujas).Returns(collectionMock.Object);

            var repo = new PujaMongoRepository(dbContextMock.Object);

            // Act
            var result = await repo.ObtenerPorSubastaAsync(subastaId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(subastaId, p.SubastaId));
        }
    }
}
