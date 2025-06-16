using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Entidades
{
    public class configuracionPujaAutomatica
    {

        public Guid Id { get; set;  }

        public string SubastaId { get; set; }

        public string UsuarioId { get; set; }

        public decimal MontoMaximo { get; set; }

        public decimal Incremento { get; set; }

        public configuracionPujaAutomatica(string subastaId, string usuarioId, decimal montoMaximo, decimal incremento)
        {
            Id = Guid.NewGuid();
            SubastaId = subastaId;
            UsuarioId = usuarioId;
            MontoMaximo = montoMaximo;
            Incremento = incremento;
        }
    }

    
}
