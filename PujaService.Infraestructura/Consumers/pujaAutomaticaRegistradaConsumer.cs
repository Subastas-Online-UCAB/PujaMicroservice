using MassTransit;
using Microsoft.Extensions.Logging;
using PujaService.Aplicacion.Eventos;
using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;

namespace PujaService.Infraestructura.Consumers
{
    public class PujaAutomaticaRegistradaConsumer : IConsumer<pujaAutomaticaRegistradaEvent>
    {
        private readonly IMongoDbContext _mongo;
        private readonly ILogger<PujaAutomaticaRegistradaConsumer> _logger;

        public PujaAutomaticaRegistradaConsumer(IMongoDbContext mongo, ILogger<PujaAutomaticaRegistradaConsumer> logger)
        {
            _mongo = mongo;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<pujaAutomaticaRegistradaEvent> context)
        {
            var ev = context.Message;

            try
            {
                var doc = new PujaMongoDto
                {
                    Id = ev.Id,
                    SubastaId = ev.SubastaId,
                    UsuarioId = ev.UsuarioId,
                    Monto = ev.Monto,
                    FechaHora = ev.FechaHora,
                    EsAutomatica = true
                };

                await _mongo.Pujas.InsertOneAsync(doc);
                _logger.LogInformation("✅ Puja automática guardada en Mongo: {Id}", ev.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar puja automática");
            }
        }
    }
}