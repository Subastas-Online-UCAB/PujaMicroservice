// Aplicacion/Handlers/RegistrarPujaAutomaticaHandler.cs
using MediatR;
using PujaService.Aplicacion.Commands;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;

namespace PujaService.Aplicacion.Handlers
{
    public class registrarPujaAutomaticaHandler : IRequestHandler<registrarPujaAutomaticaCommand, Guid>
    {
        private readonly IPujaAutomaticaRepository _repository;

        public registrarPujaAutomaticaHandler(IPujaAutomaticaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(registrarPujaAutomaticaCommand request, CancellationToken cancellationToken)
        {
            var existente = await _repository.ObtenerPorUsuarioYSubastaAsync(request.UsuarioId, request.SubastaId);

            if (existente != null)
            {
                // actualizar en lugar de duplicar
                existente.MontoMaximo = request.MontoMaximo;
                existente.Incremento = request.Incremento;
                await _repository.GuardarConfiguracionAsync(existente);
                return existente.Id;
            }

            var nuevaConfig = new configuracionPujaAutomatica(subastaId: request.SubastaId , usuarioId: request.UsuarioId, montoMaximo: request.MontoMaximo, incremento: request.Incremento);

            await _repository.GuardarConfiguracionAsync(nuevaConfig);
            return nuevaConfig.Id;
        }
    }
}