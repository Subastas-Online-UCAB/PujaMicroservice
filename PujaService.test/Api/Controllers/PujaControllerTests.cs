using Microsoft.AspNetCore.Mvc;
using Moq;
using PujaService.Api.Controllers;
using PujaService.Aplicacion.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using PujaService.Dominio.Excepciones;
using PujaService.Aplicacion.DTOs;
using PujaService.Aplicacion.Handlers;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;

namespace PujaService.test.Api.Controllers
{
    public class PujaControllerTests
    {
        [Fact]
        public async Task RegistrarPuja_CommandExitoso_ReturnsOk()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var pujaId = Guid.NewGuid();

            mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pujaId);

            var controller = new PujasController(mediatorMock.Object);

            var command = new RegistrarPujaCommand("subasta123", "usuario456", 150m);

            // Act
            var result = await controller.RegistrarPuja(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PujaResponseDto>(okResult.Value);
            Assert.Equal(pujaId, response.PujaId);
        }

        [Fact]
        public async Task RegistrarPuja_MontoInvalido_ReturnsBadRequest()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new pujasExceptions("La puja debe ser mayor a la última registrada."));

            var controller = new PujasController(mediatorMock.Object);
            var command = new RegistrarPujaCommand("subasta123", "usuario456", 50);

            // Act
            var result = await controller.RegistrarPuja(command);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }


        [Fact]
        public async Task RegistrarPuja_ErrorInesperado_ReturnsInternalServerError()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPujaCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fallo X"));

            var controller = new PujasController(mediatorMock.Object);
            var command = new RegistrarPujaCommand("subasta123", "usuario456", 50);

            // Act
            var result = await controller.RegistrarPuja(command);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
        }


        [Fact]
        public async Task Registrar_DeberiaRetornarOkConId()
        {
            // Arrange
            var mockMediator = new Mock<IMediator>();
            var controller = new PujasController(mockMediator.Object);

            var command = new registrarPujaAutomaticaCommand("subasta123", "usuario456", 150m, 2);

            var expectedId = Guid.NewGuid();
            mockMediator.Setup(m => m.Send(It.IsAny<registrarPujaAutomaticaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await controller.Registrar(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;

            // Usamos reflection para acceder a la propiedad del objeto anónimo
            var property = responseObject?.GetType().GetProperty("PujaAutomaticaId");
            var actualId = property?.GetValue(responseObject) as Guid?;

            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public async Task Handle_CuandoRepoFalla_LanzaExcepcion()
        {
            // Arrange
            var mockRepo = new Mock<IPujaAutomaticaRepository>();
            mockRepo.Setup(r => r.GuardarConfiguracionAsync(It.IsAny<configuracionPujaAutomatica>()))
                .ThrowsAsync(new Exception("Error al registrar puja automática"));

            var handler = new registrarPujaAutomaticaHandler(mockRepo.Object);

            var command = new registrarPujaAutomaticaCommand("subasta123", "usuario456", 150m, 2);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None));
        }


        [Fact]
        public async Task Registrar_CuandoCommandEsNull_DeberiaRetornarBadRequest()
        {
            var mockMediator = new Mock<IMediator>();
            var controller = new PujasController(mockMediator.Object);

            var result = await controller.Registrar(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            var value = badRequest.Value!;
            var errorProp = value.GetType().GetProperty("error")?.GetValue(value)?.ToString();

            Assert.Equal("Comando inválido o nulo", errorProp);
        }

        [Fact]
        public async Task RegistrarPujaAutomatica_Valido_RetornaOk()
        {
            var mockMediator = new Mock<IMediator>();
            var controller = new PujasController(mockMediator.Object);

            var command = new registrarPujaAutomaticaCommand("subastasss123", "ssss", 150m, 2);

            var expectedId = Guid.NewGuid();
            mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var result = await controller.Registrar(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode ?? 200);

            // Validar estructura del objeto anónimo
            var value = okResult.Value!;
            var idProp = value.GetType().GetProperty("PujaAutomaticaId");
            Assert.NotNull(idProp);
            Assert.Equal(expectedId, idProp.GetValue(value));
        }


        [Fact]
        public async Task RegistrarPujaAutomatica_CuandoMediatorFalla_Retorna500()
        {
            var mockMediator = new Mock<IMediator>();
            var controller = new PujasController(mockMediator.Object);

            var command = new registrarPujaAutomaticaCommand("subastasss123", "ssss", 150m, 2);

            mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("error simulado"));

            var result = await controller.Registrar(command);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);

            var value = statusResult.Value!;
            var error = value.GetType().GetProperty("error")?.GetValue(value)?.ToString();
            Assert.Equal("Error al registrar puja automática", error);
        }

    }
}
