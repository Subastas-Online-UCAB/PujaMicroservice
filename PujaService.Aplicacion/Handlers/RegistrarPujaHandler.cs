using MediatR;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.Eventos;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Excepciones;
using PujaService.Dominio.Interfaces;

namespace PujaServicio.Aplicacion.Handlers;

public class RegistrarPujaHandler : IRequestHandler<RegistrarPujaCommand, Guid>
{
    private readonly IPujaRepository _pujaRepository;
    private readonly IRabbitEventPublisher _eventPublisher;
    private readonly INotificadorDePujas _notificador;

    public RegistrarPujaHandler(
        IPujaRepository pujaRepository,
        IRabbitEventPublisher eventPublisher,
        INotificadorDePujas notificador)
    {
        _pujaRepository = pujaRepository;
        _eventPublisher = eventPublisher;
        _notificador = notificador;
    }

    public async Task<Guid> Handle(RegistrarPujaCommand request, CancellationToken cancellationToken)
    {
        var ultimaPuja = await _pujaRepository.ObtenerUltimaPujaAsync(request.SubastaId);

        if (ultimaPuja != null && request.Monto <= ultimaPuja.Monto)
            throw new pujasExceptions("La puja debe ser mayor a la última registrada.");

        var puja = new Puja(
            request.SubastaId, request.UsuarioId, request.Monto, false
        );

        await _pujaRepository.GuardarPujaAsync(puja);

        var evento = new PujaRegistradaEvent
        {
            Id = puja.Id,
            SubastaId = puja.SubastaId,
            UsuarioId = puja.UsuarioId,
            Monto = puja.Monto,
            FechaHora = puja.FechaHora,
            EsAutomatica = puja.EsAutomatica
        };

        await _eventPublisher.PublicarAsync(evento, cancellationToken);

        
        await _notificador.NotificarNuevaPujaAsync(
            puja.SubastaId.ToString(),
            puja.UsuarioId,
            puja.Monto,
            puja.FechaHora
        );

        return puja.Id;
    }
}