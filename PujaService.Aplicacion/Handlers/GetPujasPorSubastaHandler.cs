using MediatR;
using PujaService.Aplicacion.DTOs;
using PujaService.Aplicacion.Queries;
using PujaService.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.Handlers
{
    public class GetPujasPorSubastaHandler : IRequestHandler<GetPujasPorSubastaQuery, List<PujaDto>>
    {
        private readonly IPujaMongoRepository _pujaRepository;

        public GetPujasPorSubastaHandler(IPujaMongoRepository pujaRepository)
        {
            _pujaRepository = pujaRepository;
        }

        public async Task<List<PujaDto>> Handle(GetPujasPorSubastaQuery request, CancellationToken cancellationToken)
        {
            var pujas = await _pujaRepository.ObtenerPorSubastaAsync(request.IdSubasta);

            return pujas.Select(p => new PujaDto
            {
                Id = p.Id,
                IdSubasta = p.SubastaId,
                IdUsuario = p.UsuarioId,
                Monto = p.Monto,
                FechaPuja = p.FechaHora
            }).ToList();
        }
    }

}