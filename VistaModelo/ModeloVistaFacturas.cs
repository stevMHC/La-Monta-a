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
    public class ModeloVistaFacturas : VistaModeloBase
    {
        private readonly FacturaRepositorio _repositorio;
        private ModeloFactura _facturaSeleccionada;
        private ModeloFactura _nuevaFactura;

        public ObservableCollection<ModeloFactura> Facturas { get; set; }

        public ModeloFactura FacturaSeleccionada
        {
            get => _facturaSeleccionada;
            set
            {
                _facturaSeleccionada = value;
                OnPropertyChanged(nameof(FacturaSeleccionada));
                OnPropertyChanged(nameof(FacturaActual));
                NuevaFactura = new ModeloFactura(); // Reinicia para agregar una nueva factura
            }
        }

        public ModeloFactura NuevaFactura
        {
            get => _nuevaFactura;
            set
            {
                _nuevaFactura = value;
                OnPropertyChanged(nameof(NuevaFactura));
            }
        }

        public ModeloFactura FacturaActual => FacturaSeleccionada ?? NuevaFactura;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaFacturas()
        {
            _repositorio = new FacturaRepositorio();
            Facturas = new ObservableCollection<ModeloFactura>(_repositorio.ObtenerFacturas());
            NuevaFactura = new ModeloFactura();
            AgregarCommand = new VistaModeloDominio(AgregarFactura);
            ActualizarCommand = new VistaModeloDominio(ActualizarFactura, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarFactura, CanEliminar);
        }

        private void AgregarFactura(object parameter)
        {
            if (ValidarFactura(NuevaFactura))
            {
                _repositorio.AgregarFactura(NuevaFactura);
                CargarFacturas();
                NuevaFactura = new ModeloFactura(); // Reinicia para limpiar los campos
                OnPropertyChanged(nameof(FacturaActual));
            }
            else
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.", "Validación de Datos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool ValidarFactura(ModeloFactura factura)
        {
            return !string.IsNullOrEmpty(factura.idFactura.ToString()) &&
                   factura.idProveedor > 0 &&
                   factura.monto > 0 &&
                   !string.IsNullOrEmpty(factura.Fecha);
        }

        private void ActualizarFactura(object parameter)
        {
            if (FacturaSeleccionada != null && ValidarFactura(FacturaSeleccionada))
            {
                _repositorio.ActualizarFactura(FacturaSeleccionada);
                CargarFacturas();
            }
        }

        private bool CanActualizar(object parameter)
        {
            return FacturaSeleccionada != null;
        }

        private void EliminarFactura(object parameter)
        {
            if (FacturaSeleccionada != null)
            {
                try
                {
                    _repositorio.EliminarFactura(FacturaSeleccionada.idFactura);
                    CargarFacturas();
                    FacturaSeleccionada = null;
                    OnPropertyChanged(nameof(FacturaSeleccionada));
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
            return FacturaSeleccionada != null;
        }

        private void CargarFacturas()
        {
            Facturas.Clear();
            foreach (var factura in _repositorio.ObtenerFacturas())
            {
                Facturas.Add(factura);
            }
        }
    }
}