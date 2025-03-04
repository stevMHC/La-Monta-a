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
    public class ModeloVistaCitas : VistaModeloBase
    {
        private readonly CitaRepositorio _repositorio;
        private ModeloCitas _citaSeleccionada;
        private ModeloCitas _nuevaCita;

        public ObservableCollection<ModeloCitas> Citas { get; set; }

        public ModeloCitas CitaSeleccionada
        {
            get => _citaSeleccionada;
            set
            {
                _citaSeleccionada = value;
                OnPropertyChanged(nameof(CitaSeleccionada));
                OnPropertyChanged(nameof(CitaActual));
                // Al seleccionar una cita, reinicia NuevaCita para agregar una nueva entrada
                NuevaCita = new ModeloCitas();
            }
        }

        public ModeloCitas NuevaCita
        {
            get => _nuevaCita;
            set
            {
                _nuevaCita = value;
                OnPropertyChanged(nameof(NuevaCita));
            }
        }

        public ModeloCitas CitaActual => CitaSeleccionada ?? NuevaCita;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaCitas()
        {
            _repositorio = new CitaRepositorio();
            Citas = new ObservableCollection<ModeloCitas>(_repositorio.ObtenerCitas());
            NuevaCita = new ModeloCitas(); // Inicializa la nueva cita
            AgregarCommand = new VistaModeloDominio(AgregarCita);
            ActualizarCommand = new VistaModeloDominio(ActualizarCita, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarCita, CanEliminar);
        }

        private void AgregarCita(object parameter)
        {
            if (NuevaCita != null && ValidarCita(NuevaCita))
            {
                _repositorio.AgregarCita(NuevaCita);
                CargarCitas(); // Actualiza la lista de citas
                               // Reinicia NuevaCita para limpiar los campos
                NuevaCita = new ModeloCitas();
                OnPropertyChanged(nameof(CitaActual)); // Asegura que la vista se actualice
            }
        }

        private bool ValidarCita(ModeloCitas cita)
        {
            
            return !string.IsNullOrEmpty(cita.fecha) && !string.IsNullOrEmpty(cita.hora);
        }

        private void ActualizarCita(object parameter)
        {
            if (CitaSeleccionada != null && ValidarCita(CitaSeleccionada))
            {
                _repositorio.ActualizarCita(CitaSeleccionada);
                CargarCitas(); // Actualiza la lista de citas
            }
        }

        private bool CanActualizar(object parameter)
        {
            return CitaSeleccionada != null;
        }

        private void EliminarCita(object parameter)
        {
            if (CitaSeleccionada != null)
            {
                try
                {
                    _repositorio.EliminarCita(CitaSeleccionada.idCita);
                    CargarCitas(); // Actualiza la lista de citas
                    CitaSeleccionada = null; // Desmarca la selección después de eliminar
                    OnPropertyChanged(nameof(CitaSeleccionada));
                }
                catch (InvalidOperationException ex)
                {
                    // Muestra el mensaje de error (esto dependerá de tu implementación de la interfaz de usuario)
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEliminar(object parameter)
        {
            return CitaSeleccionada != null;
        }

        private void CargarCitas()
        {
            Citas.Clear();
            foreach (var cita in _repositorio.ObtenerCitas())
            {
                Citas.Add(cita);
            }
        }
    }
}
