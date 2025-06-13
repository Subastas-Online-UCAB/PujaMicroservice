using MediatR;
using Microsoft.AspNetCore.Mvc;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.DTOs;
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
}