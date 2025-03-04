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
    public class ModeloVistaServicios : VistaModeloBase
    {
        private readonly ServicioRepositorio _repositorio;
        private ModeloServicios _servicioSeleccionado;
        private ModeloServicios _nuevoServicio;

        public ObservableCollection<ModeloServicios> Servicios { get; set; }

        public ModeloServicios ServicioSeleccionado
        {
            get => _servicioSeleccionado;
            set
            {
                _servicioSeleccionado = value;
                OnPropertyChanged(nameof(ServicioSeleccionado));
                OnPropertyChanged(nameof(ServicioActual));
                NuevoServicio = _servicioSeleccionado != null ? null : new ModeloServicios(); // Ensure NuevoServicio is reset if no selection
            }
        }

        public ModeloServicios NuevoServicio
        {
            get => _nuevoServicio;
            set
            {
                _nuevoServicio = value;
                OnPropertyChanged(nameof(NuevoServicio));
                OnPropertyChanged(nameof(ServicioActual));
            }
        }

        public ModeloServicios ServicioActual => ServicioSeleccionado ?? NuevoServicio;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaServicios()
        {
            _repositorio = new ServicioRepositorio();
            Servicios = new ObservableCollection<ModeloServicios>(_repositorio.ObtenerServicios());
            NuevoServicio = new ModeloServicios(); // Initialize new service
            AgregarCommand = new VistaModeloDominio(AgregarServicio);
            ActualizarCommand = new VistaModeloDominio(ActualizarServicio, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarServicio, CanEliminar);
        }

        private void AgregarServicio(object parameter)
        {
            if (NuevoServicio != null && ValidarServicio(NuevoServicio))
            {
                _repositorio.AgregarServicio(NuevoServicio);
                CargarServicios(); // Update the list of services
                NuevoServicio = new ModeloServicios(); // Reset the form
            }
        }

        private bool ValidarServicio(ModeloServicios servicio)
        {
            return !string.IsNullOrEmpty(servicio.NombreServicio) &&
                   servicio.MontoServicio > 0;
        }

        private void ActualizarServicio(object parameter)
        {
            if (ServicioSeleccionado != null && ValidarServicio(ServicioSeleccionado))
            {
                _repositorio.ActualizarServicio(ServicioSeleccionado);
                CargarServicios(); // Update the list of services
            }
        }

        private bool CanActualizar(object parameter)
        {
            return ServicioSeleccionado != null;
        }

        private void EliminarServicio(object parameter)
        {
            if (ServicioSeleccionado != null)
            {
                try
                {
                    _repositorio.EliminarServicio(ServicioSeleccionado.IdServicio);
                    CargarServicios(); // Actualiza la lista de servicios
                    ServicioSeleccionado = null; // Deseleccionar después de la eliminación
                }
                catch (InvalidOperationException ex)
                {
                    // Mostrar mensaje de error al usuario
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    // Manejo genérico de errores
                    MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }




        private bool CanEliminar(object parameter)
        {
            return ServicioSeleccionado != null;
        }

        private void CargarServicios()
        {
            Servicios.Clear();
            foreach (var servicio in _repositorio.ObtenerServicios())
            {
                Servicios.Add(servicio);
            }
        }
    }
}

