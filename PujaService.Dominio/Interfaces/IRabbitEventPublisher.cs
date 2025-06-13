using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Interfaces
{
    public interface IRabbitEventPublisher
    {
        Task PublicarAsync<T>(T evento, CancellationToken cancellationToken) where T : class;
    }
}
