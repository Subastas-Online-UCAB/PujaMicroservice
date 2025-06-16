// Infraestructura/Consumers/PujaRegistradaConsumer.cs
using MassTransit;
using Microsoft.Extensions.Logging;
using PujaService.Aplicacion.Eventos;
using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;

namespace PujaService.Infraestructura.Consumers
{
    public class PujaRegistradaConsumer : IConsumer<PujaRegistradaEvent>
    {
        private readonly IMongoDbContext _context;
        private readonly ILogger<PujaRegistradaConsumer> _logger;

        public PujaRegistradaConsumer(IMongoDbContext context, ILogger<PujaRegistradaConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
        {
            var evento = context.Message;

            try
            {
                var dto = new PujaMongoDto
                {
                    Id = evento.Id,
                    SubastaId = evento.SubastaId,
                    UsuarioId = evento.UsuarioId,
                    Monto = evento.Monto,
                    FechaHora = evento.FechaHora,
                    EsAutomatica = evento.EsAutomatica
                };

                await _context.Pujas.InsertOneAsync(dto);
                _logger.LogInformation("✅ Puja registrada en Mongo: {Id}", dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar puja registrada en Mongo");
            }
        }
    }
}