using MediatR;
using PujaService.Aplicacion.Commands;
using PujaService.Aplicacion.Eventos;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Excepciones;
using PujaService.Dominio.Interfaces;

namespace PujaServicio.Aplicacion.Handlers
{

public class RegistrarPujaHandler : IRequestHandler<RegistrarPujaCommand, Guid>
{
    private readonly IPujaRepository _pujaRepository;
    private readonly IRabbitEventPublisher _eventPublisher;
    private readonly INotificadorDePujas _notificador;
    private readonly IPujaAutomaticaRepository _pujaAutomaticaRepository;

    public RegistrarPujaHandler(
        IPujaRepository pujaRepository,
        IRabbitEventPublisher eventPublisher,
        INotificadorDePujas notificador,
        IPujaAutomaticaRepository pujaAutomaticaRepository)
    {
        _pujaRepository = pujaRepository;
        _eventPublisher = eventPublisher;
        _notificador = notificador;
        _pujaAutomaticaRepository = pujaAutomaticaRepository;
    }

    public async Task<Guid> Handle(RegistrarPujaCommand request, CancellationToken cancellationToken)
    {
        var ultimaPuja = await _pujaRepository.ObtenerUltimaPujaAsync(request.SubastaId);

        if (ultimaPuja != null && request.Monto <= ultimaPuja.Monto)
            throw new pujasExceptions("La puja debe ser mayor a la última registrada.");

        var puja = new Puja(request.SubastaId, request.UsuarioId, request.Monto, false);
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
        await _notificador.NotificarNuevaPujaAsync(puja.SubastaId, puja.UsuarioId, puja.Monto, puja.FechaHora);

        // 🔁 Ejecutar lógica de puja automática en cadena
        await EjecutarPujasAutomaticasEnCadena(puja.SubastaId, puja.UsuarioId, puja.Monto, cancellationToken);

        return puja.Id;
    }

    private async Task EjecutarPujasAutomaticasEnCadena(string subastaId, string ultimoUsuario, decimal montoActual, CancellationToken cancellationToken)
    {
        var configs = await _pujaAutomaticaRepository.ObtenerConfiguracionesPorSubastaAsync(subastaId);

        var candidatas = configs
            .Where(c => c.UsuarioId != ultimoUsuario)
            .Where(c => c.MontoMaximo >= montoActual + c.Incremento)
            .OrderByDescending(c => c.MontoMaximo)
            .ToList();

        if (!candidatas.Any())
            return;

        var config = candidatas.First();

        var nuevaPuja = new Puja(subastaId, config.UsuarioId, montoActual + config.Incremento, true);
        await _pujaRepository.GuardarPujaAsync(nuevaPuja);

        var evento = new PujaRegistradaEvent
        {
            Id = nuevaPuja.Id,
            SubastaId = nuevaPuja.SubastaId,
            UsuarioId = nuevaPuja.UsuarioId,
            Monto = nuevaPuja.Monto,
            FechaHora = nuevaPuja.FechaHora,
            EsAutomatica = true
        };

        await _eventPublisher.PublicarAsync(evento, cancellationToken);
        await _notificador.NotificarNuevaPujaAsync(nuevaPuja.SubastaId, nuevaPuja.UsuarioId, nuevaPuja.Monto, nuevaPuja.FechaHora);

        // 🔁 Llamada recursiva para seguir pujando si hay más usuarios con margen
        await EjecutarPujasAutomaticasEnCadena(subastaId, nuevaPuja.UsuarioId, nuevaPuja.Monto, cancellationToken);
    }
}
}