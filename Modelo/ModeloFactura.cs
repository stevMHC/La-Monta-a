using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public class ModeloFactura
    {
        public int idFactura { get; set; }
        public int idProveedor { get; set; }
        public decimal monto { get; set; }
        public string Fecha { get; set; }

        // Propiedades para nombres
        public string nombreProvedor { get; set; }

    }
}
