using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public class ModeloInventario
    {
       
        
            public int idArticulo { get; set; }
            public int? idFactura { get; set; }
            public int idProveedor { get; set; }
            public string nombreArticulo { get; set; }
            public decimal precioCostoA { get; set; }
            public decimal precioVentaA { get; set; }
            public int cantidad { get; set; }     

        // Propiedades para nombres
           public string nombreProvedor { get; set; }       
    }
}



