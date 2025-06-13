using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using PujaService.Aplicacion.Commands;
using PujaServicio.Aplicacion.Handlers;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using Xunit;
using PujaService.Dominio.Excepciones;

namespace PujaService.test.Aplicacion.Handlers
{
    public class RegistrarPujaHandlerTests
    {
        [Fact]
        public async Task Handle_PujaMenorOIgual_ThrowMontoDePujaInvalidoException()
        {
            // Arrange
            var subastaId = Guid.NewGuid().ToString();
            var usuarioId = Guid.NewGuid().ToString();
            var montoActual = 100m;

            var ultimaPuja = new Puja(subastaId, usuarioId, montoActual, false);

            var mockRepo = new Mock<IPujaRepository>();
            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId))
                .ReturnsAsync(ultimaPuja);

            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();

            var handler = new RegistrarPujaHandler(mockRepo.Object, mockPublisher.Object, mockNotificador.Object);

            var command = new RegistrarPujaCommand(subastaId, usuarioId, 90);


            // Act & Assert
            await Assert.ThrowsAsync<pujasExceptions>(() =>
                handler.Handle(command, CancellationToken.None)
            );



        }


        [Fact]
        public async Task Handle_PujaValida_RegistraPublicaYNotificaCorrectamente()
        {
            // Arrange
            var subastaId = Guid.NewGuid().ToString();
            var usuarioId = Guid.NewGuid().ToString();
            var montoNuevo = 150m;

            var ultimaPuja = new Puja(subastaId, usuarioId, 100m, false);

            var mockRepo = new Mock<IPujaRepository>();
            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();

            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId)).ReturnsAsync(ultimaPuja);

            var handler = new RegistrarPujaHandler(mockRepo.Object, mockPublisher.Object, mockNotificador.Object);
            var command = new RegistrarPujaCommand(subastaId, usuarioId, montoNuevo);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);

            mockRepo.Verify(r => r.GuardarPujaAsync(It.IsAny<Puja>()), Times.Once);
            mockPublisher.Verify(p => p.PublicarAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
            mockNotificador.Verify(n =>
                    n.NotificarNuevaPujaAsync(subastaId, usuarioId, montoNuevo, It.IsAny<DateTime>()),
                Times.Once);
        }
    }
}
