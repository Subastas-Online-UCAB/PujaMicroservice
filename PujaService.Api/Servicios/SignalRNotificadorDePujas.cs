using Microsoft.AspNetCore.SignalR;
using PujaService.Api.Hubs;
using PujaService.Dominio.Interfaces;


namespace PujaService.Api.Servicios;

public class SignalRNotificadorDePujas : INotificadorDePujas
{
    private readonly IHubContext<PujasHub> _hub;

    public SignalRNotificadorDePujas(IHubContext<PujasHub> hub)
    {
        _hub = hub;
    }

    public async Task NotificarNuevaPujaAsync(string subastaId, string usuarioId, decimal monto, DateTime fechaHora)
    {
        await _hub.Clients.All.SendAsync("NuevaPuja", new
        {
            SubastaId = subastaId,
            UsuarioId = usuarioId,
            Monto = monto,
            FechaHora = fechaHora
        });
    }
}