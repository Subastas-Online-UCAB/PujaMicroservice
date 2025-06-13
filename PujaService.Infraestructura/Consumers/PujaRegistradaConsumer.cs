using MassTransit;
using PujaService.Aplicacion.Eventos;

using PujaService.Infraestructura.Mongo;
using PujaService.Infraestructura.Mongo.Documentos;
using PujaService.Infraestructura.Persistencia;

namespace PujaService.Infraestructura.Consumers;

public class PujaRegistradaConsumer : IConsumer<PujaRegistradaEvent>
{
    private readonly MongoDbContext _context;

    public PujaRegistradaConsumer(MongoDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<PujaRegistradaEvent> context)
    {
        var evento = context.Message;

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
    }
}