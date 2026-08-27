using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;


namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(Propietario p)
        {
            try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"INSERT INTO propietarios (nombre, apellido, dni, email, activo)
					VALUES (@nombre, @apellido, @dni, @email, @activo);
					SELECT LAST_INSERT_ID();";

					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@nombre", p.Nombre);
						command.Parameters.AddWithValue("@apellido", p.Apellido);
						command.Parameters.AddWithValue("@dni", p.Dni);
						command.Parameters.AddWithValue("@email", p.Email);
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
           try
		   {
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = "UPDATE propietarios SET activo=0 WHERE Id = @id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@id", id);
						connection.Open();
						res = command.ExecuteNonQuery();
						connection.Close();
					}
				}
				return res;
		   }
		   catch (Exception ex)
		   {
				Console.WriteLine($"Error RepositorioPropietario - Baja: {ex.Message}");
				throw;
		   }
        }

        public int Modificacion(Propietario p)
        {
            try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"UPDATE propietarios 
						SET nombre=@nombre, apellido=@apellido, dni=@dni, email=@email, activo=@activo
						WHERE id = @id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@nombre", p.Nombre);
						command.Parameters.AddWithValue("@apellido", p.Apellido);
						command.Parameters.AddWithValue("@dni", p.Dni);
						command.Parameters.AddWithValue("@email", p.Email);
						command.Parameters.AddWithValue("@activo", p.Activo);
						command.Parameters.AddWithValue("@id", p.Id);
						connection.Open();
						res = command.ExecuteNonQuery();
						connection.Close();
					}
				}
				return res;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error RepositorioPropietario - Modificacion: {ex.Message}");
				throw;
			}
        }


        public Propietario ObtenerPorId(int id)
        {
           try
		   {
				Propietario p = null;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT id, nombre, apellido, dni, email, activo
					FROM propietarios
					WHERE id=@id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							p = new Propietario
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
								Dni = reader.GetString("Dni"),
								Email = reader.GetString("Email"),
								Activo = reader.GetBoolean("Activo"),
							};
						}
						connection.Close();
					}
				}
				return p;
		   }
		   catch (Exception ex)
		   {
				Console.WriteLine($"Error RepositorioPropietario - ObtenerPorId: {ex.Message}");
				throw;
		   }
        }

    }
}