using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PujaService.Aplicacion.Commands;
using PujaServicio.Aplicacion.Handlers;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Excepciones;
using PujaService.Dominio.Interfaces;
using Xunit;
using PujaService.Tests.Seeds;

namespace PujaService.test.Aplicacion.Handlers
{
    public class RegistrarPujaHandlerTests
    {
        [Fact]
        public async Task Handle_PujaMenorOIgual_ThrowMontoDePujaInvalidoException()
        {
            // Arrange
            var subastaId = DataSeed.SubastaId;
            var usuarioId = DataSeed.UsuarioId;
            var montoBajo = 100m; // Igual o menor al de la puja registrada en el seed

            var context = DataSeed.CrearContextoEnMemoria();

            var mockRepo = new Mock<IPujaRepository>();
            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId.ToString()))
                .ReturnsAsync(new Puja(subastaId.ToString(), usuarioId.ToString(), 150m, false)); // Puja previa

            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();
            var mockAutoRepo = new Mock<IPujaAutomaticaRepository>();

            var handler = new RegistrarPujaHandler(
                mockRepo.Object,
                mockPublisher.Object,
                mockNotificador.Object,
                mockAutoRepo.Object
            );

            var command = new RegistrarPujaCommand(subastaId.ToString(), usuarioId.ToString(), montoBajo);

            // Act & Assert
            await Assert.ThrowsAsync<pujasExceptions>(() =>
                handler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_PujaValida_RegistraPublicaYNotificaCorrectamente()
        {
            // Arrange
            var subastaId = DataSeed.SubastaId;
            var usuarioId = DataSeed.UsuarioId;
            var nuevoMonto = 200m;

            var context = DataSeed.CrearContextoEnMemoria();

            var mockRepo = new Mock<IPujaRepository>();
            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();
            var mockAutoRepo = new Mock<IPujaAutomaticaRepository>();

            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId.ToString()))
                .ReturnsAsync(new Puja(subastaId.ToString(), usuarioId.ToString(), 150m, false));

            mockRepo.Setup(r => r.GuardarPujaAsync(It.IsAny<Puja>()))
                .Returns(Task.FromResult(Guid.NewGuid()));

            mockAutoRepo.Setup(r => r.ObtenerConfiguracionesPorSubastaAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<configuracionPujaAutomatica>());




            var handler = new RegistrarPujaHandler(
                mockRepo.Object,
                mockPublisher.Object,
                mockNotificador.Object,
                mockAutoRepo.Object
            );

            var command = new RegistrarPujaCommand(subastaId.ToString(), usuarioId.ToString(), nuevoMonto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            mockRepo.Verify(r => r.GuardarPujaAsync(It.IsAny<Puja>()), Times.Once);
            mockPublisher.Verify(p => p.PublicarAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
            mockNotificador.Verify(n =>
                n.NotificarNuevaPujaAsync(subastaId.ToString(), usuarioId.ToString(), nuevoMonto, It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ConPujaAutomatica_DebeEjecutarCadenaYGuardarNuevaPuja()
        {
            // Arrange
            var subastaId = DataSeed.SubastaId.ToString();
            var usuarioId = DataSeed.UsuarioId.ToString();
            var nuevoMonto = 150m;

            var configAuto = new configuracionPujaAutomatica(
                subastaId: subastaId,
                usuarioId: "otroUsuario",
                montoMaximo: 200,
                incremento: 10
            );

            var mockRepo = new Mock<IPujaRepository>();
            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();
            var mockAutoRepo = new Mock<IPujaAutomaticaRepository>();

            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId))
                .ReturnsAsync((Puja?)null); // no hay puja previa

            mockRepo.Setup(r => r.GuardarPujaAsync(It.IsAny<Puja>()))
                .Returns(Task.CompletedTask); // ✅

            mockAutoRepo.Setup(r => r.ObtenerConfiguracionesPorSubastaAsync(subastaId))
                .ReturnsAsync(new List<configuracionPujaAutomatica> { configAuto });

            var handler = new RegistrarPujaHandler(
                mockRepo.Object,
                mockPublisher.Object,
                mockNotificador.Object,
                mockAutoRepo.Object
            );

            var command = new RegistrarPujaCommand(subastaId, usuarioId, nuevoMonto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);

            // Se llama dos veces porque la automática también se guarda
            mockRepo.Verify(r => r.GuardarPujaAsync(It.IsAny<Puja>()), Times.Exactly(2));
            mockPublisher.Verify(p => p.PublicarAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            mockNotificador.Verify(n => n.NotificarNuevaPujaAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<DateTime>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_SinPujasAutomaticas_NoEjecutaCadena()
        {
            // Arrange
            var subastaId = DataSeed.SubastaId.ToString();
            var usuarioId = DataSeed.UsuarioId.ToString();
            var nuevoMonto = 200m;

            var mockRepo = new Mock<IPujaRepository>();
            var mockPublisher = new Mock<IRabbitEventPublisher>();
            var mockNotificador = new Mock<INotificadorDePujas>();
            var mockAutoRepo = new Mock<IPujaAutomaticaRepository>();

            mockRepo.Setup(r => r.ObtenerUltimaPujaAsync(subastaId))
                .ReturnsAsync((Puja?)null);

            mockRepo.Setup(r => r.GuardarPujaAsync(It.IsAny<Puja>()))
                .Returns(Task.CompletedTask); // ✅

            mockAutoRepo.Setup(r => r.ObtenerConfiguracionesPorSubastaAsync(subastaId))
                .ReturnsAsync(new List<configuracionPujaAutomatica>());

            var handler = new RegistrarPujaHandler(
                mockRepo.Object,
                mockPublisher.Object,
                mockNotificador.Object,
                mockAutoRepo.Object
            );

            var command = new RegistrarPujaCommand(subastaId, usuarioId, nuevoMonto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            mockRepo.Verify(r => r.GuardarPujaAsync(It.IsAny<Puja>()), Times.Once); // solo la puja original
        }

    }
}
