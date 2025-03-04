using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace La_Montaña.VistaModelo
{
    public abstract class VistaModeloBase: INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void  OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
