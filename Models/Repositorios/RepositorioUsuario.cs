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
			try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"INSERT INTO usuarios (nombre, apellido, email, avatar, password, rol, activo)
					VALUES (@nombre, @apellido, @email, @avatar, @password, @rol, @activo);
					SELECT LAST_INSERT_ID();";

					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@nombre", p.Nombre);
						command.Parameters.AddWithValue("@apellido", p.Apellido);
						command.Parameters.AddWithValue("@email", p.Email);
						command.Parameters.AddWithValue("@avatar", p.Avatar);
						command.Parameters.AddWithValue("@password", p.Password);
						command.Parameters.AddWithValue("@rol", 1);
						command.Parameters.AddWithValue("@activo", p.Activo);
						connection.Open();
						res = Convert.ToInt32(command.ExecuteScalar());
						p.Id = res;
						connection.Close();
					}
				}
				return res;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error RepositorioPropietario - Alta: {ex.Message}");
				throw;
			}
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

		public Usuario? ObtenerPorEmail(String email)
		{
			Usuario? p = null;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT id, nombre, apellido, email, avatar, password, rol, activo
					FROM usuarios
					WHERE email=@email";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
						p = Mapear(reader);
					}
					connection.Close();
				}
			}
			return p;
		}

		private Usuario Mapear(MySqlDataReader reader)
		{
			Usuario user = new Usuario
							{
								Id = reader.GetInt32(nameof(Usuario.Id)),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
								Email = reader.GetString("Email"),
								Avatar = reader.GetString("Avatar"),
								Password = reader.GetString("Password"),
								Rol = Enum.Parse<RolUsuario>(reader.GetString("Rol")),
				Activo = reader.GetBoolean("Activo")
							};
			return user;
		}
	}
}