using PujaService.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Interfaces
{
    public interface IPujaAutomaticaRepository
    {
        Task GuardarConfiguracionAsync(configuracionPujaAutomatica config);
        Task<List<configuracionPujaAutomatica>> ObtenerConfiguracionesPorSubastaAsync(string subastaId);
        Task<configuracionPujaAutomatica?> ObtenerPorUsuarioYSubastaAsync(string usuarioId, string subastaId);

    }
}
