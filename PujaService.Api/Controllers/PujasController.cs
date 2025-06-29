using MediatR;
using Microsoft.AspNetCore.Mvc;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.DTOs;
using PujaService.Aplicacion.Queries;
using PujaService.Dominio.Excepciones;


namespace PujaService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PujasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PujasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarPuja([FromBody] RegistrarPujaCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new PujaResponseDto { PujaId = id , msg = "Puja registrada con éxito." });
        }
        catch (pujasExceptions ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ha ocurrido un error inesperado. Intente más tarde." }); 
        }
    }
<<<<<<< Updated upstream
}
=======


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
>>>>>>> Stashed changes
