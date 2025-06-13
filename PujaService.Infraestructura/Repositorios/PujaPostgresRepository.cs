using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PujaService.Dominio.Entidades;
using PujaService.Dominio.Interfaces;
using PujaService.Infraestructura.Entidades;
using PujaService.Infraestructura.Persistencia;

namespace PujaService.Infraestructura.Repositorios
{
    public class PujaPostgresRepository : IPujaRepository
    {
        private readonly PujaDbContext _context;

        public PujaPostgresRepository(PujaDbContext context)
        {
            _context = context;
        }

        public async Task GuardarPujaAsync(Puja puja)
        {
            var entity = new PujaEntity
            {
                Id = puja.Id,
                SubastaId = puja.SubastaId,
                UsuarioId = puja.UsuarioId,
                Monto = puja.Monto,
                FechaHora = puja.FechaHora,
                EsAutomatica = puja.EsAutomatica
            };

            _context.Pujas.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Puja>> ObetenerPujasSubastaAsync(String  subastaId)
        {
            var entities = await _context.Pujas
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.FechaHora)
                .ToListAsync();

            return entities.Select(e => new Puja(
                    e.SubastaId, e.UsuarioId, e.Monto, e.EsAutomatica
                )
                { Id = e.Id, FechaHora = e.FechaHora }).ToList();
        }

        public async Task<Puja?> ObtenerUltimaPujaAsync(String subastaId)
        {
            var entity = await _context.Pujas
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.FechaHora)
                .FirstOrDefaultAsync();

            if (entity == null) return null;

            return new Puja(
                entity.SubastaId, entity.UsuarioId, entity.Monto, entity.EsAutomatica
            )
            {
                Id = entity.Id,
                FechaHora = entity.FechaHora
            };
        }
    }
}
