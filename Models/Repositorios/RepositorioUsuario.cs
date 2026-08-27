using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;


namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
            
        }

		public int Alta(Usuario p)
		{
			throw new NotImplementedException();
		}

		public int Baja(int id)
		{
			throw new NotImplementedException();
		}

		public int Modificacion(Usuario p)
		{
			throw new NotImplementedException();
		}

		public Usuario? ObtenerPorId(int id)
		{
			throw new NotImplementedException();
		}
	}
}