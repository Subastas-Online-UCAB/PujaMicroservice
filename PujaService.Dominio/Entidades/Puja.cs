using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Entidades
{
    public class Puja
    {

        public Guid Id { get; set; }
        public String SubastaId { get; set; }
        public String UsuarioId { get; set; }
        public decimal Monto { get; set; }

        public DateTime FechaHora { get; set; }

        public bool EsAutomatica { get; set; }

        public Puja() { } // 👈 NECESARIO para Mongo y para inicialización con llaves
        public Puja(String subastaId, String usuarioId, decimal monto, bool esAutomatica = false)
        {
            Id = Guid.NewGuid();
            SubastaId = subastaId;
            UsuarioId = usuarioId;
            Monto = monto;
            FechaHora = DateTime.UtcNow;
            EsAutomatica = esAutomatica;
        }
    }
}
