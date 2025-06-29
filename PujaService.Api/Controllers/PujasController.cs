using MediatR;
using Microsoft.AspNetCore.Mvc;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.DTOs;
using PujaService.Aplicacion.Queries;
using PujaService.Dominio.Excepciones;

namespace PujaService.Api.Controllers;

/// <summary>
/// Controlador encargado de manejar las operaciones relacionadas con pujas en subastas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PujasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PujasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra una nueva puja sobre una subasta activa.
    /// </summary>
    /// <param name="command">Comando con los datos de la puja (UsuarioId, SubastaId, Monto, etc.).</param>
    /// <returns>Un objeto con el ID de la puja registrada y un mensaje de éxito.</returns>
    /// <response code="200">Puja registrada exitosamente.</response>
    /// <response code="400">Error de validación o subasta no válida.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpPost("registrarPuja")]
    [ProducesResponseType(typeof(PujaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarPuja([FromBody] RegistrarPujaCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new PujaResponseDto { PujaId = id, msg = "Puja registrada con éxito." });
        }
        catch (pujasExceptions ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Ha ocurrido un error inesperado. Intente más tarde." });
        }
    }

    /// <summary>
    /// Registra una puja automática configurada por el usuario.
    /// </summary>
    /// <param name="command">Comando con los parámetros para la puja automática (UsuarioId, SubastaId, MontoMáximo, etc.).</param>
    /// <returns>El ID de la puja automática registrada.</returns>
    /// <response code="200">Puja automática registrada exitosamente.</response>
    /// <response code="400">El comando recibido es inválido o nulo.</response>
    /// <response code="500">Error interno del servidor al registrar la puja automática.</response>
    [HttpPost("registrarPujaAutomatica")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Registrar([FromBody] registrarPujaAutomaticaCommand command)
    {
        if (command == null)
            return BadRequest(new { error = "Comando inválido o nulo" });

        try
        {
            var id = await _mediator.Send(command);
            return Ok(new { PujaAutomaticaId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al registrar puja automática", detail = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las pujas registradas para una subasta específica.
    /// </summary>
    /// <param name="subastaId">ID de la subasta para la cual se desean consultar las pujas.</param>
    /// <returns>
    /// Una lista de pujas ordenadas por fecha descendente (últimas pujas primero),
    /// o un error si no se puede procesar la solicitud.
    /// </returns>
    /// <response code="200">Lista de pujas obtenida exitosamente.</response>
    /// <response code="400">La solicitud no es válida (por ejemplo, ID vacío o malformado).</response>
    /// <response code="500">Ocurrió un error interno al procesar la solicitud.</response>
    [HttpGet("subasta/{subastaId}")]
    public async Task<IActionResult> ObtenerPujasPorSubasta(string subastaId)
    {
        if (string.IsNullOrWhiteSpace(subastaId))
            return BadRequest("El ID de la subasta es obligatorio.");

        var query = new GetPujasPorSubastaQuery(subastaId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }


}
