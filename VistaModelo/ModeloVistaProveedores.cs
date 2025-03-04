using La_Montaña.Modelo;
using La_Montaña.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace La_Montaña.VistaModelo
{
    public class ModeloVistaProveedores: VistaModeloBase
    {
        private readonly ProveedorRepositorio _repositorio;
        private ModeloProveedor _proveedorSeleccionado;
        private ModeloProveedor _nuevoProveedor;

        public ObservableCollection<ModeloProveedor> Proveedores { get; set; }

        public ModeloProveedor ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set
            {
                _proveedorSeleccionado = value;
                OnPropertyChanged(nameof(ProveedorSeleccionado));
                OnPropertyChanged(nameof(ProveedorActual));
                // Al seleccionar un proveedor, reinicia NuevoProveedor para agregar una nueva entrada
                NuevoProveedor = new ModeloProveedor();
            }
        }

        public ModeloProveedor NuevoProveedor
        {
            get => _nuevoProveedor;
            set
            {
                _nuevoProveedor = value;
                OnPropertyChanged(nameof(NuevoProveedor));
            }
        }

        public ModeloProveedor ProveedorActual => ProveedorSeleccionado ?? NuevoProveedor;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaProveedores()
        {
            _repositorio = new ProveedorRepositorio();
            Proveedores = new ObservableCollection<ModeloProveedor>(_repositorio.ObtenerProveedores());
            NuevoProveedor = new ModeloProveedor(); // Inicializa el nuevo proveedor
            AgregarCommand = new VistaModeloDominio(AgregarProveedor);
            ActualizarCommand = new VistaModeloDominio(ActualizarProveedor, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarProveedor, CanEliminar);
        }

        private void AgregarProveedor(object parameter)
        {
            if (NuevoProveedor != null && ValidarProveedor(NuevoProveedor))
            {
                _repositorio.AgregarProveedor(NuevoProveedor);
                CargarProveedores(); // Actualiza la lista de proveedores
                // Reinicia NuevoProveedor para limpiar los campos
                NuevoProveedor = new ModeloProveedor();
                OnPropertyChanged(nameof(ProveedorActual)); // Asegura que la vista se actualice
            }
        }

        private bool ValidarProveedor(ModeloProveedor proveedor)
        {
            // Puedes añadir validaciones si es necesario
            return !string.IsNullOrEmpty(proveedor.nombreProvedor);
        }

        private void ActualizarProveedor(object parameter)
        {
            if (ProveedorSeleccionado != null && ValidarProveedor(ProveedorSeleccionado))
            {
                _repositorio.ActualizarProveedor(ProveedorSeleccionado);
                CargarProveedores(); // Actualiza la lista de proveedores
            }
        }

        private bool CanActualizar(object parameter)
        {
            return ProveedorSeleccionado != null;
        }

        private void EliminarProveedor(object parameter)
        {
            if (ProveedorSeleccionado != null)
            {
                try
                {
                    _repositorio.EliminarProveedor(ProveedorSeleccionado.idProveedor);
                    CargarProveedores(); // Actualiza la lista de proveedores
                    ProveedorSeleccionado = null; // Desmarca la selección después de eliminar
                    OnPropertyChanged(nameof(ProveedorSeleccionado));
                }
                catch (InvalidOperationException ex)
                {
                    // Muestra el mensaje de error al usuario
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private bool CanEliminar(object parameter)
        {
            return ProveedorSeleccionado != null;
        }

        private void CargarProveedores()
        {
            Proveedores.Clear();
            foreach (var proveedor in _repositorio.ObtenerProveedores())
            {
                Proveedores.Add(proveedor);
            }
        }
    }
}
