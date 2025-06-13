using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.Commands
{
    public class RegistrarPujaCommand : IRequest<Guid>
    {

        public String SubastaId { get; set; }
        public String UsuarioId { get; set; }
        public decimal Monto { get; set; }


        public RegistrarPujaCommand(String subastaId, String usuarioId, decimal monto)
        {
            SubastaId = subastaId;
            UsuarioId = usuarioId;
            Monto = monto;
        }
    }
}
