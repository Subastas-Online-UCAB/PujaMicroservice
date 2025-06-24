using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MongoDB.Driver;
using PujaService.Aplicacion.Eventos;
using PujaService.Infraestructura.Consumers;
using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;
using Xunit;

namespace PujaService.test.Infraestructura.Consumers
{
    public class PujaAutomaticaRegistradaConsumerTests
    {
        [Fact]
        public async Task Consume_DeberiaGuardarPujaEnMongo()
        {
            // Arrange
            var evento = new pujaAutomaticaRegistradaEvent
            {
                Id = Guid.NewGuid(),
                SubastaId = "subasta123",
                UsuarioId = "usuario456",
                Monto = 100m,
                FechaHora = DateTime.UtcNow
            };

            var mockCollection = new Mock<IMongoCollection<PujaMongoDto>>();
            mockCollection
                .Setup(x => x.InsertOneAsync(It.IsAny<PujaMongoDto>(), null, default))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mockMongoContext = new Mock<IMongoDbContext>();
            mockMongoContext.Setup(m => m.Pujas).Returns(mockCollection.Object);

            var mockLogger = new Mock<ILogger<PujaAutomaticaRegistradaConsumer>>();

            var consumer = new PujaAutomaticaRegistradaConsumer(mockMongoContext.Object, mockLogger.Object);

            var mockContext = new Mock<ConsumeContext<pujaAutomaticaRegistradaEvent>>();
            mockContext.Setup(x => x.Message).Returns(evento);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockCollection.Verify(x => x.InsertOneAsync(
                It.Is<PujaMongoDto>(p =>
                    p.Id == evento.Id &&
                    p.SubastaId == evento.SubastaId &&
                    p.UsuarioId == evento.UsuarioId &&
                    p.Monto == evento.Monto &&
                    p.EsAutomatica == true
                ),
                null,
                default), Times.Once);
        }

        [Fact]
        public async Task Consume_CuandoHayError_DeberiaRegistrarErrorEnLogger()
        {
            // Arrange
            var evento = new pujaAutomaticaRegistradaEvent
            {
                Id = Guid.NewGuid(),
                SubastaId = "sub123",
                UsuarioId = "user456",
                Monto = 150,
                FechaHora = DateTime.UtcNow
            };

            var mockCollection = new Mock<IMongoCollection<PujaMongoDto>>();
            mockCollection
                .Setup(c => c.InsertOneAsync(It.IsAny<PujaMongoDto>(), null, default))
                .ThrowsAsync(new Exception("Mongo error"));

            var mockMongoContext = new Mock<IMongoDbContext>();
            mockMongoContext.Setup(m => m.Pujas).Returns(mockCollection.Object);

            var mockLogger = new Mock<ILogger<PujaAutomaticaRegistradaConsumer>>();

            var consumer = new PujaAutomaticaRegistradaConsumer(mockMongoContext.Object, mockLogger.Object);

            var mockContext = new Mock<ConsumeContext<pujaAutomaticaRegistradaEvent>>();
            mockContext.Setup(c => c.Message).Returns(evento);

            // Act
            await consumer.Consume(mockContext.Object);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("❌ Error al guardar puja automática")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

    }
}
