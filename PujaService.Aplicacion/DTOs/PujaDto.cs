using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.DTOs
{
    public class PujaDto
    {
        public Guid Id { get; set; }
        public string IdSubasta { get; set; }
        public string IdUsuario { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPuja { get; set; }
    }
}