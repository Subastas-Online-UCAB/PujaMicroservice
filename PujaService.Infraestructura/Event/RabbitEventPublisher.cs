using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MassTransit;
using PujaService.Dominio.Interfaces;

namespace PujaServicio.Infraestructura.Servicios;

public class RabbitEventPublisher : IRabbitEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublicarAsync<T>(T evento, CancellationToken cancellationToken) where T : class
    { 
        await _publishEndpoint.Publish(evento, cancellationToken);
    }
}
