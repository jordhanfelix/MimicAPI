using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Helpers
{
    public class Paginacao
    {
        public Paginacao()
        {

        }
        public int NumeroPagina { get; set; }
        public int RegistroPagina { get; set; }
        public int TotalDeRegistro { get; set; }
        public int TotalDePagina { get; set; }

    }
}
