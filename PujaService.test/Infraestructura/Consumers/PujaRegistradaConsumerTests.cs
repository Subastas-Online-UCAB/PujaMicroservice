// Tests/Infraestructura/Consumers/PujaRegistradaConsumerTests.cs
using System;
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
    public class PujaRegistradaConsumerTests
    {
        [Fact]
        public async Task Consume_DeberiaGuardarPujaEnMongo()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<PujaMongoDto>>();
            mockCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<PujaMongoDto>(),
                null,
                default)).Returns(Task.CompletedTask);

            var mockContext = new Mock<IMongoDbContext>();
            mockContext.Setup(c => c.Pujas).Returns(mockCollection.Object);

            var mockLogger = new Mock<ILogger<PujaRegistradaConsumer>>();

            var consumer = new PujaRegistradaConsumer(mockContext.Object, mockLogger.Object);

            var evento = new PujaRegistradaEvent
            {
                Id = Guid.NewGuid(),
                SubastaId = "s123",
                UsuarioId = "u123",
                Monto = 100m,
                FechaHora = DateTime.UtcNow,
                EsAutomatica = false
            };

            var mockContexto = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            mockContexto.Setup(c => c.Message).Returns(evento);

            // Act
            await consumer.Consume(mockContexto.Object);

            // Assert
            mockCollection.Verify(c => c.InsertOneAsync(
                It.Is<PujaMongoDto>(p =>
                    p.Id == evento.Id &&
                    p.SubastaId == evento.SubastaId &&
                    p.UsuarioId == evento.UsuarioId &&
                    p.Monto == evento.Monto &&
                    p.EsAutomatica == evento.EsAutomatica
                ),
                null,
                default), Times.Once);
        }

        [Fact]
        public async Task Consume_DeberiaRegistrarErrorEnLoggerSiFallaMongo()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<PujaMongoDto>>();
            mockCollection
                .Setup(c => c.InsertOneAsync(It.IsAny<PujaMongoDto>(), null, default))
                .ThrowsAsync(new Exception("Error de prueba"));

            var mockContext = new Mock<IMongoDbContext>();
            mockContext.Setup(c => c.Pujas).Returns(mockCollection.Object);

            var mockLogger = new Mock<ILogger<PujaRegistradaConsumer>>();

            var consumer = new PujaRegistradaConsumer(mockContext.Object, mockLogger.Object);

            var evento = new PujaRegistradaEvent
            {
                Id = Guid.NewGuid(),
                SubastaId = "s-error",
                UsuarioId = "u-error",
                Monto = 999,
                FechaHora = DateTime.UtcNow,
                EsAutomatica = true
            };

            var mockConsumeContext = new Mock<ConsumeContext<PujaRegistradaEvent>>();
            mockConsumeContext.Setup(c => c.Message).Returns(evento);

            // Act
            await consumer.Consume(mockConsumeContext.Object);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Error al guardar puja")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

    }
}
