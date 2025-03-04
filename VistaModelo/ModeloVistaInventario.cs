using La_Montaña.Modelo;
using La_Montaña.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Data.SqlClient;

namespace La_Montaña.VistaModelo
{
    public class ModeloVistaInventario : VistaModeloBase
    {
        private readonly InventarioRepositorio _repositorio;
        private ModeloInventario _inventarioSeleccionado;
        private ModeloInventario _nuevoInventario;

        public ObservableCollection<ModeloInventario> Inventarios { get; set; }

        public ModeloInventario InventarioSeleccionado
        {
            get => _inventarioSeleccionado;
            set
            {
                _inventarioSeleccionado = value;
                OnPropertyChanged(nameof(InventarioSeleccionado));
                OnPropertyChanged(nameof(InventarioActual));
                NuevoInventario = new ModeloInventario(); // Reinicia para agregar un nuevo inventario
            }
        }

        public ModeloInventario NuevoInventario
        {
            get => _nuevoInventario;
            set
            {
                _nuevoInventario = value;
                OnPropertyChanged(nameof(NuevoInventario));
            }
        }

        public ModeloInventario InventarioActual => InventarioSeleccionado ?? NuevoInventario;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaInventario()
        {
            _repositorio = new InventarioRepositorio();
            Inventarios = new ObservableCollection<ModeloInventario>(_repositorio.ObtenerInventarios());
            NuevoInventario = new ModeloInventario();
            AgregarCommand = new VistaModeloDominio(AgregarInventario);
            ActualizarCommand = new VistaModeloDominio(ActualizarInventario, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarInventario, CanEliminar);
        }

        private void AgregarInventario(object parameter)
        {
            try
            {
                if (ValidarInventario(NuevoInventario))
                {
                    _repositorio.AgregarInventario(NuevoInventario);
                    CargarInventarios();
                    NuevoInventario = new ModeloInventario(); // Reinicia para limpiar los campos
                    OnPropertyChanged(nameof(InventarioActual));
                }
                else
                {
                    MessageBox.Show("Por favor, complete todos los campos requeridos.", "Validación de Datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarInventario(ModeloInventario inventario)
        {
            return !string.IsNullOrEmpty(inventario.nombreArticulo) &&
                   inventario.precioCostoA > 0 &&
                   inventario.precioVentaA > 0 &&
                   inventario.cantidad > 0;
        }

        private void ActualizarInventario(object parameter)
        {
            if (InventarioSeleccionado != null && ValidarInventario(InventarioSeleccionado))
            {
                _repositorio.ActualizarInventario(InventarioSeleccionado);
                CargarInventarios();
            }
        }

        private bool CanActualizar(object parameter)
        {
            return InventarioSeleccionado != null;
        }

        private void EliminarInventario(object parameter)
        {
            if (InventarioSeleccionado != null)
            {
                try
                {
                    _repositorio.EliminarInventario(InventarioSeleccionado.idArticulo);
                    CargarInventarios();
                    InventarioSeleccionado = null;
                    OnPropertyChanged(nameof(InventarioSeleccionado));
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEliminar(object parameter)
        {
            return InventarioSeleccionado != null;
        }

        private void CargarInventarios()
        {
            Inventarios.Clear();
            foreach (var inventario in _repositorio.ObtenerInventarios())
            {
                Inventarios.Add(inventario);
            }
        }
    }
}
