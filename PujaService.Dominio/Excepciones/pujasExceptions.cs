using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PujaService.Dominio.Excepciones
{
    public class pujasExceptions : Exception
    {
        public pujasExceptions(string msg) : base(msg) { }
    }
}
