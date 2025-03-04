using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public class ModeloCitas
    {
        public int idCita { get; set; }
        public int idCliente { get; set; }
        public int idServicio { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string Observaciones { get; set; }

        // Propiedades para nombres
        public string nombresC { get; set; }
        public string nombreServicio { get; set; }
    }
}
