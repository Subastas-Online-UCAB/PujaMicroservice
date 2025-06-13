using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Interfaces
{
    public interface INotificadorDePujas
    {
        Task NotificarNuevaPujaAsync(string subastaId, string usuarioId, decimal monto, DateTime fechaHora);
    }
}
