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

    }
}
