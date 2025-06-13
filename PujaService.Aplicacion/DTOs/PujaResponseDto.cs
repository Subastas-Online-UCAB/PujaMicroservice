using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Aplicacion.DTOs
{
    public class PujaResponseDto
    {
        public Guid PujaId { get; set; }
        public string msg { get; set;  }
    }
}
