using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;
using La_Montaña.Modelo;
using System.Data;

namespace La_Montaña.Repositorios
{
    public class RepositorioUsuario : RepositorioBase, IUsuarioRepositorio
    {
        public void Add(ModeloUsuario modeloUsuario)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using(var connection = GetConnection())
            using (var command = new SqlCommand())
            {
              connection.Open();
              command.Connection = connection;
              command.CommandText = "select*from [T_Usuario] where usuario=@usuario and [clave]=@clave";
              command.Parameters.Add("@usuario", SqlDbType.VarChar).Value = credential.UserName;
              command.Parameters.Add("@clave", SqlDbType.VarChar).Value = credential.Password;
              validUser = command.ExecuteScalar() == null ? false : true;
            }
            return validUser;
        }

        public void Edit(ModeloUsuario modeloUsuario)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ModeloUsuario> GetByAll()
        {
            throw new NotImplementedException();
        }

        public ModeloUsuario GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ModeloUsuario GetByUsuario(string usuario)
        {
            ModeloUsuario user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select nombresU,apellidosU from [T_Usuario] where usuario=@usuario";
                command.Parameters.Add("@usuario", SqlDbType.VarChar).Value = usuario;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new ModeloUsuario()
                        {
                            nombresU = reader[0].ToString(),
                            apellidosU = reader[1].ToString()
                        };
                    }
                }
            }
            return user;
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }
    }
}
