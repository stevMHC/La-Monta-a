using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome.Sharp;
using La_Montaña.Modelo;
using La_Montaña.Repositorios;

namespace La_Montaña.VistaModelo
{
    public class MainViewModel : VistaModeloBase
    {
        //Campos
        private ModeloCuentaUsuario _cuentaUsuarioActual;
        private VistaModeloBase _corrienteVistaActual;
        private string _leyenda;
        private IconChar _icono;
        private IUsuarioRepositorio repositorioUsuario;

        //propiedades

        public ModeloCuentaUsuario CuentaUsuarioActual
        {
            get
            {
                return _cuentaUsuarioActual;
            }
            set
            {
                _cuentaUsuarioActual = value;
                OnPropertyChanged(nameof(CuentaUsuarioActual));
            }
        }

        public VistaModeloBase CorrienteVistaActual
        {
            get
            {
                return _corrienteVistaActual;
            }
            set
            {
                _corrienteVistaActual = value;
                OnPropertyChanged(nameof(CorrienteVistaActual));
            }
        }

        public string Leyenda
        {
            get
            {
                return _leyenda;
            }
            set
            {
                _leyenda = value;
                OnPropertyChanged(nameof(Leyenda));
            }
        }
        
        
        public IconChar Icono
        {
            get
            {
                return _icono;
            }
            set
            {
                _icono = value;
                OnPropertyChanged(nameof(Icono));
            }
        }


        //--> Comandos para la interaccion de usuario
        public ICommand MostrarCitasComando { get; }
        public ICommand MostrarUsuariosComando { get; }
        public ICommand MostrarClientesComando { get; }
        public ICommand MostrarServiciosComando { get; }
        public ICommand MostrarProveedoresComando { get; }
        public ICommand MostrarFacturasComando { get; }
        public ICommand MostrarInventarioComando { get; }
        public ICommand MostrarIngresosComando { get; }


        public MainViewModel()
        {
            repositorioUsuario = new RepositorioUsuario();
            CuentaUsuarioActual = new ModeloCuentaUsuario();

            //Inicializar los comandos

            MostrarCitasComando = new VistaModeloDominio(ExecuteMostrarCitasComando);
            MostrarUsuariosComando = new VistaModeloDominio(ExecuteMostrarUsuariosComando);
            MostrarClientesComando = new VistaModeloDominio(ExecuteMostrarClientesComando);
            MostrarServiciosComando = new VistaModeloDominio(ExecuteMostrarServiciosComando);
            MostrarProveedoresComando = new VistaModeloDominio(ExecuteMostrarProveedoresComando);
            MostrarFacturasComando = new VistaModeloDominio(ExecuteMostrarFacturasComando);
            MostrarInventarioComando = new VistaModeloDominio(ExecuteMostrarInventarioComando);
            MostrarIngresosComando = new VistaModeloDominio(ExecuteMostrarIngresosComando);

            //Vista predeterminada.
            ExecuteMostrarCitasComando(null);

            LoadCurrentUserData();
        }

        private void ExecuteMostrarIngresosComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaIngresos();
            Leyenda = "Ingresos";
            Icono = IconChar.ColonSign;
        }

        private void ExecuteMostrarInventarioComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaInventario();
            Leyenda = "Inventario";
            Icono = IconChar.Radiation;
        }

        private void ExecuteMostrarFacturasComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaFacturas();
            Leyenda = "Facturas";
            Icono = IconChar.Receipt;
        }

        private void ExecuteMostrarProveedoresComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaProveedores();
            Leyenda = "Proveedores";
            Icono = IconChar.TruckMoving;
        }

        private void ExecuteMostrarServiciosComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaServicios();
            Leyenda = "Servicios";
            Icono = IconChar.ListDots;
        }

        private void ExecuteMostrarClientesComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaClientes();
            Leyenda = "Clientes";
            Icono = IconChar.PeopleRobbery;
        }

        private void ExecuteMostrarUsuariosComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaUsuarios();
            Leyenda = "Usuarios";
            Icono = IconChar.UserAlt;
        }

        private void ExecuteMostrarCitasComando(object obj)
        {
            CorrienteVistaActual = new ModeloVistaCitas();
            Leyenda = "Citas";
            Icono = IconChar.CalendarCheck;
        }

        private void LoadCurrentUserData()
        {
            var username = Thread.CurrentPrincipal.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                var user = repositorioUsuario.GetByUsuario(username);
                if (user != null)
                {
                    CuentaUsuarioActual.mostrarNombre= user.nombresU;
                    CuentaUsuarioActual.mostrarNombre = user.apellidosU;
                    CuentaUsuarioActual.mostrarNombre = $"{user.nombresU} {user.apellidosU}";
                }
                else
                {
                    CuentaUsuarioActual.mostrarNombre = "Usuario no válido, no ha iniciado sesión";
                    // hide child views.
                }
            }
            else
            {
                CuentaUsuarioActual.mostrarNombre = "No se ha iniciado sesión";
                // hide child views.
            }
        }
    }

}
