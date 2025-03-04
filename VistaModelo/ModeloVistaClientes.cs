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
    public class ModeloVistaClientes : VistaModeloBase
    {
        private readonly ClienteRepositorio _repositorio;
        private ModeloClientes _clienteSeleccionado;
        private ModeloClientes _nuevoCliente;

        public ObservableCollection<ModeloClientes> Clientes { get; set; }


        public ModeloClientes ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged(nameof(ClienteSeleccionado));
                OnPropertyChanged(nameof(ClienteActual));
                NuevoCliente = _clienteSeleccionado != null ? null : new ModeloClientes();
            }
        }

        public ModeloClientes NuevoCliente
        {
            get => _nuevoCliente;
            set
            {
                _nuevoCliente = value;
                OnPropertyChanged(nameof(NuevoCliente));
                OnPropertyChanged(nameof(ClienteActual));
            }
        }

        public ModeloClientes ClienteActual => ClienteSeleccionado ?? NuevoCliente;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaClientes()
        {
            _repositorio = new ClienteRepositorio();
            Clientes = new ObservableCollection<ModeloClientes>(_repositorio.ObtenerClientes());
            NuevoCliente = new ModeloClientes(); // Inicializa el nuevo cliente
            AgregarCommand = new VistaModeloDominio(AgregarCliente);
            ActualizarCommand = new VistaModeloDominio(ActualizarCliente, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarCliente, CanEliminar);
        }

        private void AgregarCliente(object parameter)
        {
            if (NuevoCliente != null && ValidarCliente(NuevoCliente))
            {
                _repositorio.AgregarCliente(NuevoCliente);
                CargarClientes(); // Actualiza la lista de clientes
                NuevoCliente = new ModeloClientes(); // Reinicia el formulario

            }
        }

        private bool ValidarCliente(ModeloClientes cliente)
        {
            return !string.IsNullOrEmpty(cliente.NombresC) &&
                   !string.IsNullOrEmpty(cliente.ApellidosC) &&
                   !string.IsNullOrEmpty(cliente.TelefonoC);
        }

        private void ActualizarCliente(object parameter)
        {
            if (ClienteSeleccionado != null && ValidarCliente(ClienteSeleccionado))
            {
                _repositorio.ActualizarCliente(ClienteSeleccionado);
                CargarClientes(); // Actualiza la lista de clientes
            }
        }

        private bool CanActualizar(object parameter)
        {
            return ClienteSeleccionado != null;
        }

        private void EliminarCliente(object parameter)
        {
            if (ClienteSeleccionado != null)
            {
                try
                {
                    _repositorio.EliminarCliente(ClienteSeleccionado.IdCliente);
                    CargarClientes(); // Actualiza la lista de clientes
                    ClienteSeleccionado = null; // Desmarca la selección después de eliminar
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private bool CanEliminar(object parameter)
        {
            return ClienteSeleccionado != null;
        }

        private void CargarClientes()
        {
            Clientes.Clear();
            foreach (var cliente in _repositorio.ObtenerClientes())
            {
                Clientes.Add(cliente);
            }
        }
    }
}
