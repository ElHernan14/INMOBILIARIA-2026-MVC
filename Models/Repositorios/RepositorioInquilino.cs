using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;


namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(Inquilino p)
        {
            try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"INSERT INTO inquilinos (nombre, apellido, dni, email, activo)
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
				Console.WriteLine($"Error RepositorioInquilino - Alta: {ex.Message}");
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
					string sql = "UPDATE inquilinos SET activo=0 WHERE Id = @id";
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
				Console.WriteLine($"Error RepositorioInquilino - Baja: {ex.Message}");
				throw;
			}
        }

        public int Modificacion(Inquilino p)
        {
           try
		   {
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"UPDATE inquilinos 
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
				Console.WriteLine($"Error RepositorioInquilino - Modificacion: {ex.Message}");
				throw;
		   }
        }


        public Inquilino? ObtenerPorId(int id)
        {
            try
			{
				Inquilino? p = null;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT id, nombre, apellido, dni, email, activo
					FROM inquilinos
					WHERE id=@id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							p = new Inquilino
							{
								Id = reader.GetInt32(nameof(Inquilino.Id)),
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
				Console.WriteLine($"Error RepositorioInquilino - ObtenerPorId: {ex.Message}");
				throw;
			}
        }

		public List<Inquilino> ObtenerTodos(bool activo = true, string? nombre = null, string? apellido = null, string? dni = null, string? email = null, int limit = 10, int page = 1)
        {
           try
		   {
                List<Inquilino> lista = [];
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sqlString = @"SELECT * FROM inquilinos WHERE activo = @activo";

					if (!string.IsNullOrWhiteSpace(nombre))
						sqlString += " AND nombre LIKE @nombre";

					if (!string.IsNullOrWhiteSpace(apellido))
						sqlString += " AND apellido LIKE @apellido";

					if (!string.IsNullOrWhiteSpace(dni))
						sqlString += " AND dni LIKE @dni";

					if (!string.IsNullOrWhiteSpace(email))
						sqlString += " AND email LIKE @email";

					sqlString += " LIMIT @limit OFFSET @offset";

					
					using (MySqlCommand command = new MySqlCommand(sqlString, connection))
					{
						command.CommandType = CommandType.Text;
						int offset = (page - 1) * limit;
						command.Parameters.AddWithValue("@activo", activo);

						if (!string.IsNullOrWhiteSpace(nombre))
							command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

						if (!string.IsNullOrWhiteSpace(apellido))
							command.Parameters.AddWithValue("@apellido", $"%{apellido}%");

						if (!string.IsNullOrWhiteSpace(dni))
							command.Parameters.AddWithValue("@dni", $"%{dni}%");

						if (!string.IsNullOrWhiteSpace(email))
							command.Parameters.AddWithValue("@email", $"%{email}%");

						command.Parameters.AddWithValue("@limit", limit);
						command.Parameters.AddWithValue("@offset", offset);
						connection.Open();
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
							Inquilino p = new Inquilino
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
								Dni = reader.GetString("Dni"),
								Email = reader.GetString("Email"),
								Activo = reader.GetBoolean("Activo"),
							};

                            lista.Add(p);
						}
						connection.Close();
					}
				}
				return lista;
		   }
		   catch (Exception ex)
		   {
				Console.WriteLine($"Error RepositorioInquilino - ObtenerTodos: {ex.Message}");
				throw;
		   }
        }

		public int ContarTodos(bool activo = true, string? nombre = null, string? apellido = null, string? dni = null, string? email = null)
		{
			try
		   {
                int total = 0;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{

					string sqlString = @"SELECT COUNT(*) FROM inquilinos WHERE activo = @activo";

					if (!string.IsNullOrWhiteSpace(nombre))
						sqlString += " AND nombre LIKE @nombre";

					if (!string.IsNullOrWhiteSpace(apellido))
						sqlString += " AND apellido LIKE @apellido";

					if (!string.IsNullOrWhiteSpace(dni))
						sqlString += " AND dni LIKE @dni";

					if (!string.IsNullOrWhiteSpace(email))
						sqlString += " AND email LIKE @email";

					using (MySqlCommand command = new MySqlCommand(sqlString, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@activo", activo);

						if (!string.IsNullOrWhiteSpace(nombre))
							command.Parameters.AddWithValue("@nombre", $"%{nombre}%");

						if (!string.IsNullOrWhiteSpace(apellido))
							command.Parameters.AddWithValue("@apellido", $"%{apellido}%");

						if (!string.IsNullOrWhiteSpace(dni))
							command.Parameters.AddWithValue("@dni", $"%{dni}%");

						if (!string.IsNullOrWhiteSpace(email))
							command.Parameters.AddWithValue("@email", $"%{email}%");

						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							total = reader.GetInt32(0);
						}
						connection.Close();
					}
				}
				return total;
		   }
		   catch (Exception ex)
		   {
				Console.WriteLine($"Error RepositorioInquilino - ContarTodos: {ex.Message}");
				throw;
		   }
		}

    }
}