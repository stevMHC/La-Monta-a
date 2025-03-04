using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using La_Montaña.Repositorios;
using System.Security.Principal;
using System.Threading;
using La_Montaña.Modelo;
using System.Net;

namespace La_Montaña.VistaModelo
{
    public class LoginVistaModelo : VistaModeloBase
    {
        //Campos
        private string _usuario;
        private SecureString _clave;
        private string _mensajeError;
        private bool _isViewVisible = true;

        private IUsuarioRepositorio usuarioRepositorio;

        //propiedades
        public string Usuario
        {
            get
            { 
                return _usuario;
            } 
            set 
            { 
                _usuario = value; 
                OnPropertyChanged(nameof(Usuario));
            } 
        }
        public SecureString Clave
        {
            get
            {
                return _clave;
               
            }
            set
            {
                _clave = value;
                OnPropertyChanged(nameof(Clave));
            }
        }
        public string MensajeError
        {
            get
            {
                return _mensajeError;
                
            }
            set
            {
                _mensajeError = value;
                OnPropertyChanged(nameof(MensajeError));
            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
                
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        // -> Comandos

        public ICommand LoginComando { get; }
        public ICommand RecuperarClaveComando { get; }
        public ICommand MostrarClaveComando { get; }
        public ICommand RecordarClaveComando { get; }

        //Constructor
        public LoginVistaModelo()
        {
            usuarioRepositorio = new RepositorioUsuario();
            LoginComando = new VistaModeloDominio(ExecuteLoginComando, CanExecuteLoginComando);
            RecuperarClaveComando = new VistaModeloDominio(p => ExecuteRecuperarClaveCamando("",""));
        }



        private bool CanExecuteLoginComando(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(Usuario) || Usuario.Length < 3 ||
                Clave == null || Clave.Length < 3)
                validData = false;
            else
                validData = true;
            return validData;
        }

        private void ExecuteLoginComando(object obj)
        {
            var isValidUser = usuarioRepositorio.AuthenticateUser(new NetworkCredential(Usuario, Clave));
            if (isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(Usuario), null);
                IsViewVisible = false;
            }
            else
            {
                MensajeError = "* Usuario o clave invalidos";
            }
        }

        private void ExecuteRecuperarClaveCamando(string usuario, string correo)
        {
            throw new NotImplementedException();
        }

    }
}
