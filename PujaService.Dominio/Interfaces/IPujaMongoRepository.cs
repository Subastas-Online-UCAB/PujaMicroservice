using PujaService.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Interfaces
{
    public interface IPujaMongoRepository
    {
        Task<List<Puja>> ObtenerPorSubastaAsync(string idSubasta);
    }
}
