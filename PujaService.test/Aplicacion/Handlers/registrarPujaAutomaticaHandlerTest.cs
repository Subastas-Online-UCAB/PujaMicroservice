using Moq;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.Handlers;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.test.Aplicacion.Handlers
{
    public class registrarPujaAutomaticaHandlerTest
    {

        [Fact]
        public async Task Handle_CuandoNoExisteConfiguracion_DeberiaCrearUnaNueva()
        {
            // Arrange
            var mockRepo = new Mock<IPujaAutomaticaRepository>();

            mockRepo.Setup(r => r.ObtenerPorUsuarioYSubastaAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((configuracionPujaAutomatica?)null);

            configuracionPujaAutomatica? configCapturada = null;

            mockRepo.Setup(r => r.GuardarConfiguracionAsync(It.IsAny<configuracionPujaAutomatica>()))
                .Callback<configuracionPujaAutomatica>(c => configCapturada = c)
                .Returns<configuracionPujaAutomatica>(c => Task.FromResult(c.Id)); // usamos el mismo ID real

            var handler = new registrarPujaAutomaticaHandler(mockRepo.Object);

            var command = new registrarPujaAutomaticaCommand("subastasss123", "ssss", 150m, 2);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(configCapturada);
            Assert.Equal(configCapturada!.Id, result); // ✅ ahora sí coincide
        }


        [Fact]
        public async Task Handle_CuandoYaExisteConfiguracion_DeberiaActualizarYRetornarMismoId()
        {
            // Arrange
            var configExistente = new configuracionPujaAutomatica("s1", "u1", 300, 15);

            var mockRepo = new Mock<IPujaAutomaticaRepository>();
            mockRepo.Setup(r => r.ObtenerPorUsuarioYSubastaAsync("u1", "s1"))
                .ReturnsAsync(configExistente);

            mockRepo.Setup(r => r.GuardarConfiguracionAsync(It.IsAny<configuracionPujaAutomatica>()))
                .Returns<configuracionPujaAutomatica>(c => Task.FromResult(c.Id));

            var handler = new registrarPujaAutomaticaHandler(mockRepo.Object);

            var command = new registrarPujaAutomaticaCommand("s1", "u1", 999m, 88);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(configExistente.Id, result);
            Assert.Equal(999, configExistente.MontoMaximo);
            Assert.Equal(88, configExistente.Incremento);

            mockRepo.Verify(r => r.GuardarConfiguracionAsync(It.IsAny<configuracionPujaAutomatica>()), Times.Once);
        }


        [Fact]
        public async Task Handle_CuandoGuardarLanzaExcepcion_DeberiaPropagarla()
        {
            var mockRepo = new Mock<IPujaAutomaticaRepository>();

            mockRepo.Setup(r => r.ObtenerPorUsuarioYSubastaAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((configuracionPujaAutomatica?)null);

            mockRepo.Setup(r => r.GuardarConfiguracionAsync(It.IsAny<configuracionPujaAutomatica>()))
                .ThrowsAsync(new Exception("Error simulado"));

            var handler = new registrarPujaAutomaticaHandler(mockRepo.Object);

            var command = new registrarPujaAutomaticaCommand("subastasss123", "ssss", 150m, 2);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None));
        }


    }
}
