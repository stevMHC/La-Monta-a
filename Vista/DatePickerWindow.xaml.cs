using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace La_Montaña.Vista
{
    /// <summary>
    /// Lógica de interacción para DatePickerWindow.xaml
    /// </summary>
    public partial class DatePickerWindow : Window
    {
        public DateTime FechaSeleccionada { get; private set; }
        public string TipoReporteSeleccionado { get; private set; }

        public DatePickerWindow()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FechaSeleccionada = datePicker.SelectedDate.GetValueOrDefault();
            TipoReporteSeleccionado = (reportTypeComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
            DialogResult = true;
            Close();
        }
    }
}
