using La_Montaña.Modelo;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml; // Asegúrate de agregar una referencia a EPPlus
using La_Montaña.Modelo; // Asegúrate de usar el espacio de nombres correcto para ModeloIngresos



namespace La_Montaña.Reportes
{
    public class ReporteGenerador
    {
        public ReporteGenerador()
        {
            // Establecer el contexto de licencia
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // O LicenseContext.NonCommercial dependiendo de tu uso
        }

        public void GenerarReporteDiario(IEnumerable<ModeloIngresos> ingresos, string rutaArchivo)
        {
            using (var package = new ExcelPackage())
            {
                // Implementación para generar el reporte en Excel
                var worksheet = package.Workbook.Worksheets.Add("Ingresos");
                // Agregar datos a la hoja de cálculo...

                // Guardar el archivo
                package.SaveAs(new FileInfo(rutaArchivo));
            }

            // Verifica si la ruta del archivo es válida
            if (string.IsNullOrEmpty(rutaArchivo))
            {
                throw new ArgumentException("La ruta del archivo no puede ser nula o vacía.", nameof(rutaArchivo));
            }

            // Crea un nuevo paquete Excel
            using (var package = new ExcelPackage())
            {
                // Agrega una nueva hoja de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Reporte Diario");

                // Define los encabezados de las columnas
                worksheet.Cells[1, 1].Value = "ID Ingreso";
                worksheet.Cells[1, 2].Value = "ID Usuario";
                worksheet.Cells[1, 3].Value = "ID Cliente";
                worksheet.Cells[1, 4].Value = "ID Cita";
                worksheet.Cells[1, 5].Value = "ID Artículo";
                worksheet.Cells[1, 6].Value = "ID Servicio";
                worksheet.Cells[1, 7].Value = "Fecha";
                worksheet.Cells[1, 8].Value = "Monto";

                // Llena la hoja con los datos
                int row = 2; // Comienza en la segunda fila para dejar la primera fila para encabezados
                foreach (var ingreso in ingresos)
                {
                    worksheet.Cells[row, 1].Value = ingreso.idIngreso;
                    worksheet.Cells[row, 2].Value = ingreso.idUsuario;
                    worksheet.Cells[row, 3].Value = ingreso.idCliente;
                    worksheet.Cells[row, 4].Value = ingreso.idCita;
                    worksheet.Cells[row, 5].Value = ingreso.idArticulo;
                    worksheet.Cells[row, 6].Value = ingreso.idServicio;
                    worksheet.Cells[row, 7].Value = ingreso.Fecha; // Asegúrate de tener una propiedad de Fecha en ModeloIngresos
                    worksheet.Cells[row, 8].Value = ingreso.monto; // Asegúrate de tener una propiedad de Monto en ModeloIngresos
                    row++;
                }

                // Ajusta automáticamente el tamaño de las columnas
                worksheet.Cells.AutoFitColumns();

                // Guarda el archivo en la ruta especificada
                File.WriteAllBytes(rutaArchivo, package.GetAsByteArray());
            }
        }

        public void GenerarReporteSemanal(IEnumerable<ModeloIngresos> ingresos, string rutaArchivo, DateTime fechaInicio)
        {
            using (var package = new ExcelPackage())
            {
                // Crear una hoja de cálculo
                var worksheet = package.Workbook.Worksheets.Add("Ingresos Semanales");

                // Agregar encabezados de columnas
                worksheet.Cells["A1"].Value = "ID Ingreso";
                worksheet.Cells["B1"].Value = "Fecha";
                worksheet.Cells["C1"].Value = "Monto";
                worksheet.Cells["D1"].Value = "Descripción";

                // Agregar datos
                int row = 2;
                foreach (var ingreso in ingresos)
                {
                    worksheet.Cells[$"A{row}"].Value = ingreso.idIngreso;
                    worksheet.Cells[$"B{row}"].Value = ingreso.Fecha;
                    worksheet.Cells[$"C{row}"].Value = ingreso.monto;
                    worksheet.Cells[$"D{row}"].Value = ingreso.Descripcion;
                    row++;
                }

                // Guardar el archivo
                package.SaveAs(new FileInfo(rutaArchivo));
            }
        }

        public void GenerarReporteMensual(IEnumerable<ModeloIngresos> ingresos, string rutaArchivo, DateTime fechaInicio)
        {
            using (var package = new ExcelPackage())
            {
                // Crear una hoja de cálculo
                var worksheet = package.Workbook.Worksheets.Add("Ingresos Mensuales");

                // Agregar encabezados de columnas
                worksheet.Cells["A1"].Value = "ID Ingreso";
                worksheet.Cells["B1"].Value = "Fecha";
                worksheet.Cells["C1"].Value = "Monto";
                worksheet.Cells["D1"].Value = "Descripción";

                // Agregar datos
                int row = 2;
                foreach (var ingreso in ingresos)
                {
                    worksheet.Cells[$"A{row}"].Value = ingreso.idIngreso;
                    worksheet.Cells[$"B{row}"].Value = ingreso.Fecha;
                    worksheet.Cells[$"C{row}"].Value = ingreso.monto;
                    worksheet.Cells[$"D{row}"].Value = ingreso.Descripcion;
                    row++;
                }

                // Guardar el archivo
                package.SaveAs(new FileInfo(rutaArchivo));
            }
        }
    }
}
