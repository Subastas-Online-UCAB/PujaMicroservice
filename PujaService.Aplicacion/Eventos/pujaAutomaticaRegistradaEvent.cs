using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.Eventos
{
    public class pujaAutomaticaRegistradaEvent
    {
        public Guid Id { get; set; }
        public string SubastaId { get; set; } = null!;
        public string UsuarioId { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
