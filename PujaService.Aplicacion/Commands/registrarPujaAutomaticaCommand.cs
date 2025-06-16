using MediatR;

namespace PujaService.Aplicacion.Commands
{
    public class registrarPujaAutomaticaCommand : IRequest<Guid>
    {
        public string SubastaId { get; set; }
        public string UsuarioId { get; set; }
        public decimal MontoMaximo { get; set; }
        public decimal Incremento { get; set; }

        public registrarPujaAutomaticaCommand(string subastaId, string usuarioId, decimal montoMaximo, decimal incremento)
        {
            SubastaId = subastaId;
            UsuarioId = usuarioId;
            MontoMaximo = montoMaximo;
            Incremento = incremento;
        }

    }
}