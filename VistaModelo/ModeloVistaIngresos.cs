using La_Montaña.Modelo;
using La_Montaña.Repositorios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.IO;
using La_Montaña.Reportes;
using La_Montaña.Vista;
namespace La_Montaña.VistaModelo
{
    public class ModeloVistaIngresos : VistaModeloBase
    {
        private readonly RepositorioIngreso _repositorio;
        private ModeloIngresos _ingresoSeleccionado;
        private ModeloIngresos _nuevoIngreso;
        private string _mensajeValidacion;
        private ICommand _exportarReporteCommand;

        public ObservableCollection<ModeloIngresos> Ingresos { get; set; }

        public ModeloIngresos IngresoSeleccionado
        {
            get => _ingresoSeleccionado;
            set
            {
                _ingresoSeleccionado = value;
                OnPropertyChanged(nameof(IngresoSeleccionado));
                OnPropertyChanged(nameof(IngresoActual));
                NuevoIngreso = new ModeloIngresos(); // Reinicia para agregar un nuevo ingreso
            }
        }

        public ModeloIngresos NuevoIngreso
        {
            get => _nuevoIngreso;
            set
            {
                _nuevoIngreso = value;
                OnPropertyChanged(nameof(NuevoIngreso));
            }
        }

        public ModeloIngresos IngresoActual => IngresoSeleccionado ?? NuevoIngreso;

        public ICommand AgregarCommand { get; }
        public ICommand ActualizarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand ExportarReporteCommand
        {
            get
            {
                return _exportarReporteCommand ?? (_exportarReporteCommand = new VistaModeloDominio(
                    param => AbrirVentanaSeleccionarFecha()));
            }
        }

        public string MensajeValidacion
        {
            get => _mensajeValidacion;
            set
            {
                _mensajeValidacion = value;
                OnPropertyChanged(nameof(MensajeValidacion));
            }
        }

        public ModeloVistaIngresos()
        {
            _repositorio = new RepositorioIngreso();
            Ingresos = new ObservableCollection<ModeloIngresos>(_repositorio.ObtenerIngresos());
            NuevoIngreso = new ModeloIngresos();
            AgregarCommand = new VistaModeloDominio(AgregarIngreso);
            ActualizarCommand = new VistaModeloDominio(ActualizarIngreso, CanActualizar);
            EliminarCommand = new VistaModeloDominio(EliminarIngreso, CanEliminar);
        }

        private void AgregarIngreso(object parameter)
        {
            if (ValidarIngreso(NuevoIngreso, out var mensaje))
            {
                _repositorio.AgregarIngreso(NuevoIngreso);
                CargarIngresos();
                NuevoIngreso = new ModeloIngresos(); // Reinicia para limpiar los campos
                OnPropertyChanged(nameof(IngresoActual));
                MensajeValidacion = string.Empty; // Limpiar mensaje
            }
            else
            {
                MensajeValidacion = mensaje;
            }
        }

        private bool ValidarIngreso(ModeloIngresos ingreso, out string mensaje)
        {
            // Inicializa el mensaje vacío por defecto
            mensaje = string.Empty;

            if (!_repositorio.VerificarExistenciaUsuario(ingreso.idUsuario))
            {
                mensaje += "El Usuario no existe.\n";
            }

            if (!_repositorio.VerificarExistenciaCliente(ingreso.idCliente))
            {
                mensaje += "El Cliente no existe.\n";
            }

            if (ingreso.idCita.HasValue && !_repositorio.VerificarExistenciaCita(ingreso.idCita.Value))
            {
                mensaje += "La Cita no existe.\n";
            }

            if (ingreso.idArticulo.HasValue && !_repositorio.VerificarExistenciaArticulo(ingreso.idArticulo.Value))
            {
                mensaje += "El Artículo no existe.\n";
            }

            if (ingreso.idServicio.HasValue && !_repositorio.VerificarExistenciaServicio(ingreso.idServicio.Value))
            {
                mensaje += "El Servicio no existe.\n";
            }

            return string.IsNullOrEmpty(mensaje);
        }

        private bool CanActualizar(object parameter)
        {
            return IngresoSeleccionado != null;
        }

        private void ActualizarIngreso(object parameter)
        {
            if (IngresoSeleccionado != null)
            {
                if (ValidarIngreso(IngresoSeleccionado, out var mensaje))
                {
                    _repositorio.ActualizarIngreso(IngresoSeleccionado);
                    CargarIngresos();
                    MensajeValidacion = string.Empty; // Limpiar mensaje si la actualización es exitosa
                }
                else
                {
                    MensajeValidacion = mensaje; // Mostrar mensaje de error si hay problemas
                }
            }
        }

        private bool CanEliminar(object parameter)
        {
            return IngresoSeleccionado != null;
        }

        private void EliminarIngreso(object parameter)
        {
            if (IngresoSeleccionado != null)
            {
                _repositorio.EliminarIngreso(IngresoSeleccionado.idIngreso);
                CargarIngresos();
                IngresoSeleccionado = null;
                OnPropertyChanged(nameof(IngresoSeleccionado));
            }
        }

        private void CargarIngresos()
        {
            Ingresos.Clear();
            foreach (var ingreso in _repositorio.ObtenerIngresos())
            {
                Ingresos.Add(ingreso);
            }
        }

        private void AbrirVentanaSeleccionarFecha()
        {
            DatePickerWindow ventana = new DatePickerWindow();
            ventana.Owner = Application.Current.MainWindow; // Establece la ventana principal como propietario
            if (ventana.ShowDialog() == true) // Muestra la ventana como un diálogo modal
            {
                var fechaSeleccionada = ventana.FechaSeleccionada; // Obtiene la fecha seleccionada
                var tipoReporteSeleccionado = ventana.TipoReporteSeleccionado; // Obtiene el tipo de reporte seleccionado
                ExportarReporte(fechaSeleccionada, tipoReporteSeleccionado);
            }
        }

        private void ExportarReporte(DateTime fecha, string tipoReporte)
        {
            string rutaArchivo;
            var ingresos = new List<ModeloIngresos>(); // Suponiendo que Ingreso es el tipo de datos

            switch (tipoReporte)
            {
                case "Daily":
                    ingresos = _repositorio.ObtenerIngresosPorFecha(fecha);
                    rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Reporte_Ingresos_Diarios_{fecha:yyyyMMdd}.xlsx");
                    var reporteGeneradorDiario = new ReporteGenerador();
                    reporteGeneradorDiario.GenerarReporteDiario(ingresos, rutaArchivo);
                    break;

                case "Weekly":
                    var fechaInicio = fecha.AddDays(-(int)fecha.DayOfWeek); // Primer día de la semana
                    var fechaFin = fechaInicio.AddDays(6); // Último día de la semana
                    ingresos = _repositorio.ObtenerIngresosPorPeriodo(fechaInicio, fechaFin);
                    rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Reporte_Ingresos_Semanales_{fechaInicio:yyyyMMdd}_a_{fechaFin:yyyyMMdd}.xlsx");
                    var reporteGeneradorSemanal = new ReporteGenerador();
                    reporteGeneradorSemanal.GenerarReporteSemanal(ingresos, rutaArchivo, fechaInicio);
                    break;

                case "Monthly":
                    var fechaInicioMes = new DateTime(fecha.Year, fecha.Month, 1); // Primer día del mes
                    var fechaFinMes = fechaInicioMes.AddMonths(1).AddDays(-1); // Último día del mes
                    ingresos = _repositorio.ObtenerIngresosPorPeriodo(fechaInicioMes, fechaFinMes);
                    rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Reporte_Ingresos_Mensuales_{fechaInicioMes:yyyyMM}.xlsx");
                    var reporteGeneradorMensual = new ReporteGenerador();
                    reporteGeneradorMensual.GenerarReporteMensual(ingresos, rutaArchivo, fechaInicioMes);
                    break;

                default:
                    MessageBox.Show("Tipo de reporte no reconocido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            MessageBox.Show($"El reporte se ha generado exitosamente en: {rutaArchivo}", "Reporte Generado", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
