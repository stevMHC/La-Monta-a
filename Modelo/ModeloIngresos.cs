using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public class ModeloIngresos
    {
        public int idIngreso { get; set; }
        public int idUsuario { get; set; }
        public int idCliente { get; set; }
        public int? idCita { get; set; }
        public int? idArticulo { get; set; }
        public int? idServicio { get; set; }
        public int cantidad { get; set; }
        public string formaPago { get; set; }
        public string Fecha { get; set; }
        public string Observaciones { get; set; }
        public decimal monto { get; set; }

        public decimal Descripcion { get; set; }

        // Propiedades para nombres
        public string nombresU { get; set; }
        public string nombresC { get; set; }
        public string nombreArticulo { get; set; }
        public string nombreServicio { get; set; }
    }
}
