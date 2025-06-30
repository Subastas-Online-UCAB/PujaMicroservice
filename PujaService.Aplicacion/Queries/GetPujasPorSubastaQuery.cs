using MediatR;
using PujaService.Aplicacion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.Queries
{
    public class GetPujasPorSubastaQuery : IRequest<List<PujaDto>>
    {
        public string IdSubasta { get; set; }

        public GetPujasPorSubastaQuery(string idSubasta)
        {
            IdSubasta = idSubasta;
        }
    }
}