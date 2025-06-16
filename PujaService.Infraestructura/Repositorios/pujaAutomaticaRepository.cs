// Infraestructura/Persistencia/Repositorios/PujaAutomaticaPostgresRepository.cs
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using PujaService.Infraestructura.Persistencia;

namespace PujaService.Infraestructura.Repositorios
{
    public class PujaAutomaticaPostgresRepository : IPujaAutomaticaRepository
    {
        private readonly PujaDbContext _context;

        public PujaAutomaticaPostgresRepository(PujaDbContext context)
        {
            _context = context;
        }

        public async Task GuardarConfiguracionAsync(configuracionPujaAutomatica config)
        {
            var existente = await _context.PujasAutomaticas
                .FirstOrDefaultAsync(p => p.UsuarioId == config.UsuarioId && p.SubastaId == config.SubastaId);

            if (existente == null)
                _context.PujasAutomaticas.Add(config);
            else
                _context.PujasAutomaticas.Update(config);

            await _context.SaveChangesAsync();
        }

        public async Task<List<configuracionPujaAutomatica>> ObtenerConfiguracionesPorSubastaAsync(string subastaId)
        {
            return await _context.PujasAutomaticas
                .Where(p => p.SubastaId == subastaId)
                .ToListAsync();
        }

        public async Task<configuracionPujaAutomatica?> ObtenerPorUsuarioYSubastaAsync(string usuarioId, string subastaId)
        {
            return await _context.PujasAutomaticas
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.SubastaId == subastaId);
        }
    }
}