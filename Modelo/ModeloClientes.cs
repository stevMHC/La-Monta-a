using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public class ModeloClientes : INotifyPropertyChanged
    {
        private int _idCliente;
        private string _nombresC;
        private string _apellidosC;
        private string _telefonoC;

        public int IdCliente
        {
            get => _idCliente;
            set
            {
                _idCliente = value;
                OnPropertyChanged(nameof(IdCliente));
            }
        }

        public string NombresC
        {
            get => _nombresC;
            set
            {
                _nombresC = value;
                OnPropertyChanged(nameof(NombresC));
            }
        }

        public string ApellidosC
        {
            get => _apellidosC;
            set
            {
                _apellidosC = value;
                OnPropertyChanged(nameof(ApellidosC));
            }
        }

        public string TelefonoC
        {
            get => _telefonoC;
            set
            {
                _telefonoC = value;
                OnPropertyChanged(nameof(TelefonoC));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
