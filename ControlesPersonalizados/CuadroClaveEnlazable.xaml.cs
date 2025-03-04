using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace La_Montaña.ControlesPersonalizados
{
    /// <summary>
    /// Lógica de interacción para CuadroClaveEnlazable.xaml
    /// </summary>
    public partial class CuadroClaveEnlazable : UserControl
    {
        public static readonly DependencyProperty Paswordproperty =
            DependencyProperty.Register("Clave", typeof(SecureString), typeof(CuadroClaveEnlazable));

        public SecureString Clave
        {
            get { return (SecureString)GetValue(Paswordproperty); }
            set { SetValue(Paswordproperty, value); }
        }

        public CuadroClaveEnlazable()
        {
            InitializeComponent();
            txtClave.PasswordChanged += OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
           Clave = txtClave.SecurePassword;
        }
    }
}
