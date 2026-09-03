using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;


namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
    {
        public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(TipoInmueble p)
        {
            try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"INSERT INTO tipos_inmueble (nombre, descripcion)
					VALUES (@nombre, @descripcion);
					SELECT LAST_INSERT_ID();";

					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@nombre", p.Nombre);
						command.Parameters.AddWithValue("@descripcion", p.Descripcion);
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
				Console.WriteLine($"Error RepositorioTipoInmueble - Alta: {ex.Message}");
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
					string sql = "UPDATE tipos_inmueble SET activo=0 WHERE Id = @id";
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
				Console.WriteLine($"Error RepositorioTipoInmueble - Baja: {ex.Message}");
				throw;
		   }
        }

        public int Modificacion(TipoInmueble p)
        {
            try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"UPDATE tipos_inmueble 
						SET nombre=@nombre, descripcion=@descripcion
						WHERE id = @id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@nombre", p.Nombre);
						command.Parameters.AddWithValue("@apellido", p.Descripcion);
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
				Console.WriteLine($"Error RepositorioTipoInmueble - Modificacion: {ex.Message}");
				throw;
			}
        }


        public TipoInmueble ObtenerPorId(int id)
        {
           try
		   {
				TipoInmueble p = null;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT id, nombre, descripcion, activo
					FROM tipos_inmueble
					WHERE id=@id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							p = new TipoInmueble
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString("Nombre"),
								Descripcion = reader.GetString("Descripcion"),
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
				Console.WriteLine($"Error RepositorioTipoInmueble - ObtenerPorId: {ex.Message}");
				throw;
		   }
        }

        public List<TipoInmueble> ObtenerTodos()
        {
           try
		   {
                List<TipoInmueble> lista = [];
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT * FROM tipos_inmueble WHERE activo = 1";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{

						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
							TipoInmueble p = new TipoInmueble
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString("Nombre"),
								Descripcion = reader.GetString("Descripcion"),
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
				Console.WriteLine($"Error RepositorioTipoInmueble - ObtenerTodos: {ex.Message}");
				throw;
		   }
        }

    }
}