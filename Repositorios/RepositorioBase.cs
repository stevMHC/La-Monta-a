using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using La_Montaña.Modelo;
using System.Windows;
using System.Data;
using FontAwesome.Sharp;


namespace La_Montaña.Repositorios
{
    public abstract class RepositorioBase
    {
        private readonly string _connectionString;
        public RepositorioBase()
        {
            _connectionString = "Server=DESKTOP-74P3BUO\\UNIVERSIDA_D; Database=La_MontañaBD; Integrated Security=true";
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }

    public class UsuarioRepositorio : RepositorioBase
    {
        public IEnumerable<ModeloUsuario> ObtenerUsuarios()
        {
            var usuarios = new List<ModeloUsuario>();

            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM T_Usuario", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new ModeloUsuario
                    {
                        Id = reader.GetInt32(0),
                        nombresU = reader.GetString(1),
                        apellidosU = reader.GetString(2),
                        telefonoU = reader.GetString(3),
                        fechaNacimientoU = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                        correoU = reader.GetString(5),
                        usuario = reader.GetString(6),
                        clave = reader.GetString(7)
                    });
                }
            }

            return usuarios;
        }

        public void AgregarUsuario(ModeloUsuario usuario)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO T_Usuario (nombresU, apellidosU, telefonoU, fechaNacimientoU, correoU, usuario, clave) VALUES (@nombresU, @apellidosU, @telefonoU, @fechaNacimientoU, @correoU, @usuario, @clave)",
                    connection);

                command.Parameters.AddWithValue("@nombresU", usuario.nombresU);
                command.Parameters.AddWithValue("@apellidosU", usuario.apellidosU);
                command.Parameters.AddWithValue("@telefonoU", usuario.telefonoU);
                command.Parameters.AddWithValue("@fechaNacimientoU", usuario.fechaNacimientoU);
                command.Parameters.AddWithValue("@correoU", usuario.correoU);
                command.Parameters.AddWithValue("@usuario", usuario.usuario);
                command.Parameters.AddWithValue("@clave", usuario.clave);

                command.ExecuteNonQuery();
            }
        }

        public void ActualizarUsuario(ModeloUsuario usuario)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new SqlCommand(
                    "UPDATE T_Usuario SET nombresU = @nombresU, apellidosU = @apellidosU, telefonoU = @telefonoU, fechaNacimientoU = @fechaNacimientoU, correoU = @correoU, usuario = @usuario, clave = @clave WHERE idUsuario = @idUsuario",
                    connection);

                command.Parameters.AddWithValue("@nombresU", usuario.nombresU);
                command.Parameters.AddWithValue("@apellidosU", usuario.apellidosU);
                command.Parameters.AddWithValue("@telefonoU", usuario.telefonoU);
                command.Parameters.AddWithValue("@fechaNacimientoU", usuario.fechaNacimientoU);
                command.Parameters.AddWithValue("@correoU", usuario.correoU);
                command.Parameters.AddWithValue("@usuario", usuario.usuario);
                command.Parameters.AddWithValue("@clave", usuario.clave);
                command.Parameters.AddWithValue("@idUsuario", usuario.Id);

                command.ExecuteNonQuery();
            }
        }

        public void EliminarUsuario(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var command = new SqlCommand("EliminarUsuario", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@idUsuario", id);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Manejo del error si el usuario tiene ingresos registrados
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }

    public class ClienteRepositorio : RepositorioBase
    {
        public List<ModeloClientes> ObtenerClientes()
        {
            var clientes = new List<ModeloClientes>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT idCliente, nombresC, apellidosC, telefonoC FROM T_Cliente", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientes.Add(new ModeloClientes
                            {
                                IdCliente = reader.GetInt32(0),
                                NombresC = reader.GetString(1),
                                ApellidosC = reader.GetString(2),
                                TelefonoC = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return clientes;
        }

        public void AgregarCliente(ModeloClientes cliente)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO T_Cliente (nombresC, apellidosC, telefonoC) VALUES (@NombresC, @ApellidosC, @TelefonoC)", connection))
                {
                    command.Parameters.AddWithValue("@NombresC", cliente.NombresC);
                    command.Parameters.AddWithValue("@ApellidosC", cliente.ApellidosC);
                    command.Parameters.AddWithValue("@TelefonoC", cliente.TelefonoC);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarCliente(ModeloClientes cliente)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE T_Cliente SET nombresC = @NombresC, apellidosC = @ApellidosC, telefonoC = @TelefonoC WHERE idCliente = @IdCliente", connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    command.Parameters.AddWithValue("@NombresC", cliente.NombresC);
                    command.Parameters.AddWithValue("@ApellidosC", cliente.ApellidosC);
                    command.Parameters.AddWithValue("@TelefonoC", cliente.TelefonoC);

                    command.ExecuteNonQuery();
                }
            }
        }

        public bool PuedeEliminarCliente(int idCliente)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT CASE
                WHEN EXISTS (SELECT 1 FROM T_Citas WHERE idCliente = @IdCliente) THEN 0
                WHEN EXISTS (SELECT 1 FROM T_Ingreso WHERE idCliente = @IdCliente) THEN 0
                ELSE 1
            END", connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", idCliente);

                    var result = (int)command.ExecuteScalar();
                    return result == 1;
                }
            }
        }

        public void EliminarCliente(int idCliente)
        {
            if (PuedeEliminarCliente(idCliente))
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "DELETE FROM T_Cliente WHERE idCliente = @IdCliente", connection))
                    {
                        command.Parameters.AddWithValue("@IdCliente", idCliente);
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No se puede eliminar el cliente porque tiene registros en citas o ingresos.");
            }
        }

    }

    public class ServicioRepositorio : RepositorioBase
    {
        public List<ModeloServicios> ObtenerServicios()
        {
            var servicios = new List<ModeloServicios>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM T_Servicio", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            servicios.Add(new ModeloServicios
                            {
                                IdServicio = reader.GetInt32(0),
                                NombreServicio = reader.GetString(1),
                                MontoServicio = reader.GetDecimal(2)
                            });
                        }
                    }
                }
            }

            return servicios;
        }

        public void AgregarServicio(ModeloServicios servicio)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO T_Servicio (nombreServicio, montoServicio) VALUES (@nombre, @monto)", connection))
                {
                    command.Parameters.AddWithValue("@nombre", servicio.NombreServicio);
                    command.Parameters.AddWithValue("@monto", servicio.MontoServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarServicio(ModeloServicios servicio)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE T_Servicio SET nombreServicio = @nombre, montoServicio = @monto WHERE idServicio = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", servicio.IdServicio);
                    command.Parameters.AddWithValue("@nombre", servicio.NombreServicio);
                    command.Parameters.AddWithValue("@monto", servicio.MontoServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool TieneCitasAsociadas(int idServicio)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Citas WHERE idServicio = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", idServicio);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void EliminarServicio(int idServicio)
        {
            if (TieneCitasAsociadas(idServicio))
            {
                throw new InvalidOperationException("No se puede eliminar el servicio porque hay citas registradas con este servicio.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM T_Servicio WHERE idServicio = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", idServicio);
                    command.ExecuteNonQuery();
                }
            }
        }
    }


    public class CitaRepositorio : RepositorioBase
    {
        public List<ModeloCitas> ObtenerCitas()
        {
            var citas = new List<ModeloCitas>();
            var today = DateTime.Today;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT 
                c.idCita, 
                c.idCliente, 
                c.idServicio, 
                c.fecha, 
                c.hora, 
                c.Observaciones,
                cli.nombresC,
                ser.nombreServicio
            FROM T_Citas c
            INNER JOIN T_Cliente cli ON c.idCliente = cli.idCliente
            INNER JOIN T_Servicio ser ON c.idServicio = ser.idServicio
            ORDER BY 
                CASE 
                    WHEN c.fecha = @today THEN 1 
                    WHEN c.fecha > @today THEN 2 
                    ELSE 3 
                END,
                CASE 
                    WHEN c.fecha = @today THEN c.hora 
                    ELSE NULL 
                END,
                c.fecha, 
                c.hora", connection))
                {
                    command.Parameters.AddWithValue("@today", today);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            citas.Add(new ModeloCitas
                            {
                                idCita = reader.GetInt32(0),
                                idCliente = reader.GetInt32(1),
                                idServicio = reader.GetInt32(2),
                                fecha = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                hora = reader.GetString(4),
                                Observaciones = reader.GetString(5),
                                nombresC = reader.GetString(6),
                                nombreServicio = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return citas;
        }

        public void AgregarCita(ModeloCitas cita)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO T_Citas (idCliente, idServicio, fecha, hora, Observaciones) VALUES (@idCliente, @idServicio, @fecha, @hora, @observaciones)", connection))
                {
                    command.Parameters.AddWithValue("@idCliente", cita.idCliente);
                    command.Parameters.AddWithValue("@idServicio", cita.idServicio);
                    command.Parameters.AddWithValue("@fecha", cita.fecha);
                    command.Parameters.AddWithValue("@hora", cita.hora);
                    command.Parameters.AddWithValue("@observaciones", cita.Observaciones);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarCita(ModeloCitas cita)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE T_Citas SET idCliente = @idCliente, idServicio = @idServicio, fecha = @fecha, hora = @hora, Observaciones = @observaciones WHERE idCita = @idCita", connection))
                {
                    command.Parameters.AddWithValue("@idCita", cita.idCita);
                    command.Parameters.AddWithValue("@idCliente", cita.idCliente);
                    command.Parameters.AddWithValue("@idServicio", cita.idServicio);
                    command.Parameters.AddWithValue("@fecha", cita.fecha);
                    command.Parameters.AddWithValue("@hora", cita.hora);
                    command.Parameters.AddWithValue("@observaciones", cita.Observaciones);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool TieneIngresosAsociados(int idCita)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Ingreso WHERE idCita = @idCita", connection))
                {
                    command.Parameters.AddWithValue("@idCita", idCita);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void EliminarCita(int idCita)
        {
            if (TieneIngresosAsociados(idCita))
            {
                throw new InvalidOperationException("No se puede eliminar la cita porque hay ingresos registrados con esta cita.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM T_Citas WHERE idCita = @idCita", connection))
                {
                    command.Parameters.AddWithValue("@idCita", idCita);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class ProveedorRepositorio : RepositorioBase
    {
        public List<ModeloProveedor> ObtenerProveedores()
        {
            var proveedores = new List<ModeloProveedor>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT 
                idProveedor, 
                nombreProvedor, 
                telefonoProvedor, 
                DireccionProvedor
            FROM T_Provedor", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            proveedores.Add(new ModeloProveedor
                            {
                                idProveedor = reader.GetInt32(0),
                                nombreProvedor = reader.GetString(1),
                                telefonoProvedor = reader.GetString(2),
                                DireccionProvedor = reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return proveedores;
        }

        public void AgregarProveedor(ModeloProveedor proveedor)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO T_Provedor (nombreProvedor, telefonoProvedor, DireccionProvedor) VALUES (@NombreProvedor, @TelefonoProvedor, @DireccionProvedor)", connection))
                {
                    command.Parameters.AddWithValue("@NombreProvedor", proveedor.nombreProvedor);
                    command.Parameters.AddWithValue("@TelefonoProvedor", proveedor.telefonoProvedor);
                    command.Parameters.AddWithValue("@DireccionProvedor", proveedor.DireccionProvedor);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarProveedor(ModeloProveedor proveedor)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE T_Provedor SET nombreProvedor = @NombreProvedor, telefonoProvedor = @TelefonoProvedor, DireccionProvedor = @DireccionProvedor WHERE idProveedor = @IdProveedor", connection))
                {
                    command.Parameters.AddWithValue("@IdProveedor", proveedor.idProveedor);
                    command.Parameters.AddWithValue("@NombreProvedor", proveedor.nombreProvedor);
                    command.Parameters.AddWithValue("@TelefonoProvedor", proveedor.telefonoProvedor);
                    command.Parameters.AddWithValue("@DireccionProvedor", proveedor.DireccionProvedor);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool PuedeEliminarProveedor(int idProveedor)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // Verifica en T_Factura
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Factura WHERE idProveedor = @IdProveedor", connection))
                {
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0) return false;
                }

                // Verifica en T_Inventario
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Inventario WHERE idProveedor = @IdProveedor", connection))
                {
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0) return false;
                }
            }

            return true;
        }

        public void EliminarProveedor(int idProveedor)
        {
            if (!PuedeEliminarProveedor(idProveedor))
            {
                throw new InvalidOperationException("No se puede eliminar el proveedor porque existen registros en  el modulo de facturas o inventario.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM T_Provedor WHERE idProveedor = @IdProveedor", connection))
                {
                    command.Parameters.AddWithValue("@IdProveedor", idProveedor);
                    command.ExecuteNonQuery();
                }
            }
        }
    }


    public class FacturaRepositorio : RepositorioBase
    {
        public List<ModeloFactura> ObtenerFacturas()
        {
            var facturas = new List<ModeloFactura>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT 
                f.idFactura, 
                f.idProveedor, 
                f.monto, 
                f.Fecha,
                p.nombreProvedor
            FROM T_Factura f
            INNER JOIN T_Provedor p ON f.idProveedor = p.idProveedor", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            facturas.Add(new ModeloFactura
                            {
                                idFactura = reader.GetInt32(0),
                                idProveedor = reader.GetInt32(1),
                                monto = reader.GetDecimal(2),
                                Fecha = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                nombreProvedor = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return facturas;
        }

        public void AgregarFactura(ModeloFactura factura)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO T_Factura (idFactura, idProveedor, monto, Fecha) VALUES (@idFactura, @idProveedor, @monto, @Fecha)", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", factura.idFactura);
                    command.Parameters.AddWithValue("@idProveedor", factura.idProveedor);
                    command.Parameters.AddWithValue("@monto", factura.monto);
                    command.Parameters.AddWithValue("@Fecha", factura.Fecha);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarFactura(ModeloFactura factura)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE T_Factura SET idProveedor = @idProveedor, monto = @monto, Fecha = @Fecha WHERE idFactura = @idFactura", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", factura.idFactura);
                    command.Parameters.AddWithValue("@idProveedor", factura.idProveedor);
                    command.Parameters.AddWithValue("@monto", factura.monto);
                    command.Parameters.AddWithValue("@Fecha", factura.Fecha);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool PuedeEliminarFactura(int idFactura)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // Verifica en T_Inventario
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Inventario WHERE idFactura = @idFactura", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", idFactura);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0) return false;
                }
            }

            return true;
        }

        public void EliminarFactura(int idFactura)
        {
            if (!PuedeEliminarFactura(idFactura))
            {
                throw new InvalidOperationException("No se puede eliminar la factura porque existen registros en el módulo de inventarios.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                // Primero actualiza las dependencias
                using (var command = new SqlCommand("UPDATE T_Inventario SET idFactura = NULL WHERE idFactura = @idFactura", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", idFactura);
                    command.ExecuteNonQuery();
                }

                // Luego elimina la factura
                using (var command = new SqlCommand("DELETE FROM T_Factura WHERE idFactura = @idFactura", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", idFactura);
                    command.ExecuteNonQuery();
                }
            }
        }
    }


    public class InventarioRepositorio : RepositorioBase
    {
        public List<ModeloInventario> ObtenerInventarios()
        {
            var inventarios = new List<ModeloInventario>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
            SELECT 
                i.idArticulo, 
                i.idFactura, 
                i.idProveedor, 
                i.nombreArticulo, 
                i.precioCostoA, 
                i.precioVentaA, 
                i.cantidad,
                p.nombreProvedor
            FROM T_Inventario i
            INNER JOIN T_Provedor p ON i.idProveedor = p.idProveedor", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventarios.Add(new ModeloInventario
                            {
                                idArticulo = reader.GetInt32(0),
                                idFactura = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                idProveedor = reader.GetInt32(2),
                                nombreArticulo = reader.GetString(3),
                                precioCostoA = reader.GetDecimal(4),
                                precioVentaA = reader.GetDecimal(5),
                                cantidad = reader.GetInt32(6),
                                nombreProvedor = reader.GetString(7)
                            });
                        }
                    }
                }
            }

            return inventarios;
        }

        public bool FacturaExiste(int idFactura)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Factura WHERE idFactura = @idFactura", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", idFactura);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public bool InventarioTieneRegistrosEnIngresos(int idArticulo)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Ingreso WHERE idArticulo = @idArticulo", connection))
                {
                    command.Parameters.AddWithValue("@idArticulo", idArticulo);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public void AgregarInventario(ModeloInventario inventario)
        {
            if (inventario.idFactura.HasValue && !FacturaExiste(inventario.idFactura.Value))
            {
                throw new ArgumentException("El número de factura proporcionado no existe.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO T_Inventario (idFactura, idProveedor, nombreArticulo, precioCostoA, precioVentaA, cantidad) VALUES (@idFactura, @idProveedor, @nombreArticulo, @precioCostoA, @precioVentaA, @cantidad)", connection))
                {
                    command.Parameters.AddWithValue("@idFactura", (object)inventario.idFactura ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idProveedor", inventario.idProveedor);
                    command.Parameters.AddWithValue("@nombreArticulo", inventario.nombreArticulo);
                    command.Parameters.AddWithValue("@precioCostoA", inventario.precioCostoA);
                    command.Parameters.AddWithValue("@precioVentaA", inventario.precioVentaA);
                    command.Parameters.AddWithValue("@cantidad", inventario.cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarInventario(ModeloInventario inventario)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE T_Inventario SET idFactura = @idFactura, idProveedor = @idProveedor, nombreArticulo = @nombreArticulo, precioCostoA = @precioCostoA, precioVentaA = @precioVentaA, cantidad = @cantidad WHERE idArticulo = @idArticulo", connection))
                {
                    command.Parameters.AddWithValue("@idArticulo", inventario.idArticulo);
                    command.Parameters.AddWithValue("@idFactura", (object)inventario.idFactura ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idProveedor", inventario.idProveedor);
                    command.Parameters.AddWithValue("@nombreArticulo", inventario.nombreArticulo);
                    command.Parameters.AddWithValue("@precioCostoA", inventario.precioCostoA);
                    command.Parameters.AddWithValue("@precioVentaA", inventario.precioVentaA);
                    command.Parameters.AddWithValue("@cantidad", inventario.cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarInventario(int idArticulo)
        {
            if (InventarioTieneRegistrosEnIngresos(idArticulo))
            {
                throw new InvalidOperationException("No se puede eliminar el artículo porque existen registros en el módulo de ingresos.");
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM T_Inventario WHERE idArticulo = @idArticulo", connection))
                {
                    command.Parameters.AddWithValue("@idArticulo", idArticulo);
                    command.ExecuteNonQuery();
                }
            }
        }
    }


    public class RepositorioIngreso : RepositorioBase
    {
        // Método para obtener todos los ingresos
        public List<ModeloIngresos> ObtenerIngresos()
        {
            var ingresos = new List<ModeloIngresos>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                SELECT 
                    i.idIngreso,
                    i.idUsuario,
                    i.idCliente,
                    i.idCita,
                    i.idArticulo,
                    i.idServicio,
                    i.cantidad,
                    i.formaPago,
                    i.Fecha,
                    i.monto,
                    u.nombresU,
                    c.nombresC,
                    a.nombreArticulo,
                    s.nombreServicio
                FROM T_Ingreso i
                INNER JOIN T_Usuario u ON i.idUsuario = u.idUsuario
                INNER JOIN T_Cliente c ON i.idCliente = c.idCliente
                LEFT JOIN T_Inventario a ON i.idArticulo = a.idArticulo
                LEFT JOIN T_Servicio s ON i.idServicio = s.idServicio
                ORDER BY i.Fecha DESC", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingresos.Add(new ModeloIngresos
                            {
                                idIngreso = reader.GetInt32(0),
                                idUsuario = reader.GetInt32(1),
                                idCliente = reader.GetInt32(2),
                                idCita = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                idArticulo = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                idServicio = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                cantidad = reader.GetInt32(6),
                                formaPago = reader.GetString(7),
                                Fecha = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                monto = reader.GetDecimal(9),
                                nombresU = reader.GetString(10),
                                nombresC = reader.GetString(11),
                                nombreArticulo = reader.IsDBNull(12) ? null : reader.GetString(12),
                                nombreServicio = reader.IsDBNull(13) ? null : reader.GetString(13)
                            });
                        }
                    }
                }
            }

            return ingresos;
        }

        // Método para obtener ingresos por fecha específica
        public List<ModeloIngresos> ObtenerIngresosPorFecha(DateTime fecha)
        {
            var ingresos = new List<ModeloIngresos>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                SELECT 
                    i.idIngreso,
                    i.idUsuario,
                    i.idCliente,
                    i.idCita,
                    i.idArticulo,
                    i.idServicio,
                    i.cantidad,
                    i.formaPago,
                    i.Fecha,
                    i.monto,
                    u.nombresU,
                    c.nombresC,
                    a.nombreArticulo,
                    s.nombreServicio
                FROM T_Ingreso i
                INNER JOIN T_Usuario u ON i.idUsuario = u.idUsuario
                INNER JOIN T_Cliente c ON i.idCliente = c.idCliente
                LEFT JOIN T_Inventario a ON i.idArticulo = a.idArticulo
                LEFT JOIN T_Servicio s ON i.idServicio = s.idServicio
                WHERE CAST(i.Fecha AS DATE) = @fecha
                ORDER BY i.Fecha DESC", connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha.Date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingresos.Add(new ModeloIngresos
                            {
                                idIngreso = reader.GetInt32(0),
                                idUsuario = reader.GetInt32(1),
                                idCliente = reader.GetInt32(2),
                                idCita = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                idArticulo = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                idServicio = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                cantidad = reader.GetInt32(6),
                                formaPago = reader.GetString(7),
                                Fecha = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                monto = reader.GetDecimal(9),
                                nombresU = reader.GetString(10),
                                nombresC = reader.GetString(11),
                                nombreArticulo = reader.IsDBNull(12) ? null : reader.GetString(12),
                                nombreServicio = reader.IsDBNull(13) ? null : reader.GetString(13)
                            });
                        }
                    }
                }
            }

            return ingresos;
        }

        // Método para obtener ingresos por periodo
        public List<ModeloIngresos> ObtenerIngresosPorPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            var ingresos = new List<ModeloIngresos>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                SELECT 
                    i.idIngreso,
                    i.idUsuario,
                    i.idCliente,
                    i.idCita,
                    i.idArticulo,
                    i.idServicio,
                    i.cantidad,
                    i.formaPago,
                    i.Fecha,
                    i.monto,
                    u.nombresU,
                    c.nombresC,
                    a.nombreArticulo,
                    s.nombreServicio
                FROM T_Ingreso i
                INNER JOIN T_Usuario u ON i.idUsuario = u.idUsuario
                INNER JOIN T_Cliente c ON i.idCliente = c.idCliente
                LEFT JOIN T_Inventario a ON i.idArticulo = a.idArticulo
                LEFT JOIN T_Servicio s ON i.idServicio = s.idServicio
                WHERE i.Fecha BETWEEN @fechaInicio AND @fechaFin
                ORDER BY i.Fecha DESC", connection))
                {
                    command.Parameters.AddWithValue("@fechaInicio", fechaInicio.Date);
                    command.Parameters.AddWithValue("@fechaFin", fechaFin.Date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ingresos.Add(new ModeloIngresos
                            {
                                idIngreso = reader.GetInt32(0),
                                idUsuario = reader.GetInt32(1),
                                idCliente = reader.GetInt32(2),
                                idCita = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                idArticulo = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                idServicio = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                cantidad = reader.GetInt32(6),
                                formaPago = reader.GetString(7),
                                Fecha = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                monto = reader.GetDecimal(9),
                                nombresU = reader.GetString(10),
                                nombresC = reader.GetString(11),
                                nombreArticulo = reader.IsDBNull(12) ? null : reader.GetString(12),
                                nombreServicio = reader.IsDBNull(13) ? null : reader.GetString(13)
                            });
                        }
                    }
                }
            }

            return ingresos;
        }

        // Otros métodos del repositorio
        public void AgregarIngreso(ModeloIngresos ingreso)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                INSERT INTO T_Ingreso (idUsuario, idCliente, idCita, idArticulo, idServicio, cantidad, formaPago, Fecha, monto)
                VALUES (@idUsuario, @idCliente, @idCita, @idArticulo, @idServicio, @cantidad, @formaPago, @Fecha, @monto)", connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", ingreso.idUsuario);
                    command.Parameters.AddWithValue("@idCliente", ingreso.idCliente);
                    command.Parameters.AddWithValue("@idCita", (object)ingreso.idCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idArticulo", (object)ingreso.idArticulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idServicio", (object)ingreso.idServicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cantidad", ingreso.cantidad);
                    command.Parameters.AddWithValue("@formaPago", ingreso.formaPago);
                    command.Parameters.AddWithValue("@Fecha", ingreso.Fecha);
                    command.Parameters.AddWithValue("@monto", (object)ingreso.monto ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarIngreso(ModeloIngresos ingreso)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(@"
                UPDATE T_Ingreso
                SET idUsuario = @idUsuario,
                    idCliente = @idCliente,
                    idCita = @idCita,
                    idArticulo = @idArticulo,
                    idServicio = @idServicio,
                    cantidad = @cantidad,
                    formaPago = @formaPago,
                    Fecha = @Fecha,
                    monto = @monto
                WHERE idIngreso = @idIngreso", connection))
                {
                    command.Parameters.AddWithValue("@idIngreso", ingreso.idIngreso);
                    command.Parameters.AddWithValue("@idUsuario", ingreso.idUsuario);
                    command.Parameters.AddWithValue("@idCliente", ingreso.idCliente);
                    command.Parameters.AddWithValue("@idCita", (object)ingreso.idCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idArticulo", (object)ingreso.idArticulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@idServicio", (object)ingreso.idServicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@cantidad", ingreso.cantidad);
                    command.Parameters.AddWithValue("@formaPago", ingreso.formaPago);
                    command.Parameters.AddWithValue("@Fecha", ingreso.Fecha);
                    command.Parameters.AddWithValue("@monto", (object)ingreso.monto ?? DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarIngreso(int idIngreso)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM T_Ingreso WHERE idIngreso = @idIngreso", connection))
                {
                    command.Parameters.AddWithValue("@idIngreso", idIngreso);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool VerificarExistenciaUsuario(int idUsuario)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Usuario WHERE idUsuario = @idUsuario", connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool VerificarExistenciaCliente(int idCliente)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Cliente WHERE idCliente = @idCliente", connection))
                {
                    command.Parameters.AddWithValue("@idCliente", idCliente);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool VerificarExistenciaCita(int idCita)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Citas WHERE idCita = @idCita", connection))
                {
                    command.Parameters.AddWithValue("@idCita", idCita);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool VerificarExistenciaArticulo(int idArticulo)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Inventario WHERE idArticulo = @idArticulo", connection))
                {
                    command.Parameters.AddWithValue("@idArticulo", idArticulo);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public bool VerificarExistenciaServicio(int idServicio)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM T_Servicio WHERE idServicio = @idServicio", connection))
                {
                    command.Parameters.AddWithValue("@idServicio", idServicio);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }
    }



}



