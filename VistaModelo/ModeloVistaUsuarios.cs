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
    public class ModeloVistaUsuarios : VistaModeloBase
    {
        private readonly UsuarioRepositorio _repositorio;
        private ModeloUsuario _usuarioSeleccionado;
        private ModeloUsuario _nuevoUsuario;

        public ObservableCollection<ModeloUsuario> Usuarios { get; set; }

        public ModeloUsuario UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                _usuarioSeleccionado = value;
                OnPropertyChanged(nameof(UsuarioSeleccionado));
                OnPropertyChanged(nameof(UsuarioActual));
                NuevoUsuario = _usuarioSeleccionado != null ? null : new ModeloUsuario();
            }
        }

        public ModeloUsuario NuevoUsuario
        {
            get => _nuevoUsuario;
            set
            {
                _nuevoUsuario = value;
                OnPropertyChanged(nameof(NuevoUsuario));
                OnPropertyChanged(nameof(UsuarioActual));
            }
        }

        public ModeloUsuario UsuarioActual => UsuarioSeleccionado ?? NuevoUsuario;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }

        public ModeloVistaUsuarios()
        {
            _repositorio = new UsuarioRepositorio();
            Usuarios = new ObservableCollection<ModeloUsuario>(_repositorio.ObtenerUsuarios());
            NuevoUsuario = new ModeloUsuario(); // Inicializa el nuevo usuario
            AgregarCommand = new VistaModeloDominio(AgregarUsuario);
            ActualizarCommand = new VistaModeloDominio(ActualizarUsuario, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarUsuario, CanEliminar);
        }

        private void AgregarUsuario(object parameter)
        {
            if (NuevoUsuario != null && ValidarUsuario(NuevoUsuario))
            {
                _repositorio.AgregarUsuario(NuevoUsuario);
                CargarUsuarios(); // Actualiza la lista de usuarios
                NuevoUsuario = new ModeloUsuario(); // Reinicia el formulario
            }
        }

        private bool ValidarUsuario(ModeloUsuario usuario)
        {
            return !string.IsNullOrEmpty(usuario.nombresU) &&
                   !string.IsNullOrEmpty(usuario.apellidosU) &&
                   !string.IsNullOrEmpty(usuario.telefonoU) &&
                   !string.IsNullOrEmpty(usuario.fechaNacimientoU) &&
                   !string.IsNullOrEmpty(usuario.correoU) &&
                   !string.IsNullOrEmpty(usuario.usuario) &&
                   !string.IsNullOrEmpty(usuario.clave);
        }

        private void ActualizarUsuario(object parameter)
        {
            if (UsuarioSeleccionado != null && ValidarUsuario(UsuarioSeleccionado))
            {
                _repositorio.ActualizarUsuario(UsuarioSeleccionado);
                CargarUsuarios(); // Actualiza la lista de usuarios
            }
        }

        private bool CanActualizar(object parameter)
        {
            return UsuarioSeleccionado != null;
        }

        private void EliminarUsuario(object parameter)
        {
            if (UsuarioSeleccionado != null)
            {
                try
                {
                    _repositorio.EliminarUsuario(UsuarioSeleccionado.Id);
                    CargarUsuarios(); // Actualiza la lista de usuarios
                    UsuarioSeleccionado = null; // Desmarca la selección después de eliminar
                }
                catch (Exception ex)
                {
                    // Mostrar el mensaje de error al usuario
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEliminar(object parameter)
        {
            return UsuarioSeleccionado != null;
        }

        private void CargarUsuarios()
        {
            Usuarios.Clear();
            foreach (var usuario in _repositorio.ObtenerUsuarios())
            {
                Usuarios.Add(usuario);
            }
        }
    }

}

