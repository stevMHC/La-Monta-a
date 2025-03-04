using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace La_Montaña.Modelo
{
    public interface IUsuarioRepositorio
    {
        bool AuthenticateUser (NetworkCredential credential);
        void Add(ModeloUsuario modeloUsuario);
        void Edit(ModeloUsuario modeloUsuario);
        void Remove(int id);
        ModeloUsuario GetById(int id);
        ModeloUsuario GetByUsuario(string usuario);
        IEnumerable<ModeloUsuario> GetByAll();

        //...
    }
}
