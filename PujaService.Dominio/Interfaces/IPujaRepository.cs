using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PujaService.Dominio.Entidades;

namespace PujaService.Dominio.Interfaces
{
    public interface IPujaRepository
    {
        Task GuardarPujaAsync(Puja puja);
        Task<List<Puja>> ObetenerPujasSubastaAsync(String subastaId);
        Task<Puja?> ObtenerUltimaPujaAsync(String subastaId);
    }
}
