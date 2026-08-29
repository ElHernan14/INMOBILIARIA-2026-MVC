using INMOBILIARIA.Models.Interfaces;
//using INMOBILIARIA.Models.Repositorios;
using MySqlConnector;
using System.Data;

namespace INMOBILIARIA.Models.Repositorios
{
	public class RepositorioReserva : RepositorioBase, IRepositorioReserva
	{
		private readonly IRepositorioInmueble Inmuebles;
		private readonly IRepositorioInquilino Inquilinos;
		private readonly IRepositorioUsuario Usuarios;

		public RepositorioReserva(IRepositorioInmueble repoInmueble, IRepositorioInquilino repoInquilino, IRepositorioUsuario repoUsuario, IConfiguration configuration) : base(configuration)
		{
			this.Inmuebles = repoInmueble;
			this.Inquilinos = repoInquilino;
			this.Usuarios = repoUsuario;
		}	

		public int Alta(Reserva p) 
		{
			try
			{
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"INSERT INTO reservas (inmueble_id, inquilino_id, usuario_creador_id, usuario_cancelador_id, fecha_desde, fecha_hasta, cancelada, fecha_creacion, fecha_cancelacion)
					VALUES (@inmueble, @inquilino, @usuario_creador, @usuario_cancelador, @fecha_desde, @fecha_hasta, @cancelada, @fecha_creacion, @fecha_cancelacion);
					SELECT LAST_INSERT_ID();";

					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@inmueble", p.Inmueble?.Id);
						command.Parameters.AddWithValue("@inquilino", p.Inquilino?.Id);
						command.Parameters.AddWithValue("@usuario_creador", p.UsuarioCreador?.Id);
						command.Parameters.AddWithValue("@usuario_cancelador", p.UsuarioCancelador?.Id);
						command.Parameters.AddWithValue("@fecha_desde", p.FechaDesde);
						command.Parameters.AddWithValue("@fecha_hasta", p.FechaHasta);
						command.Parameters.AddWithValue("@cancelada", !p.Activo);
						command.Parameters.AddWithValue("@fecha_creacion", p.FechaCreacion);
						command.Parameters.AddWithValue("@fecha_cancelacion", p.FechaCancelacion);
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
				Console.WriteLine($"Error RepositorioReserva - Alta: {ex.Message}");
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
					string sql = "UPDATE reservas SET cancelada=TRUE WHERE Id = @id";
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
				Console.WriteLine($"Error RepositorioReserva - Baja: {ex.Message}");
				throw;
			}
		}

		public int Modificacion(Reserva p) 
		{
			try
		   {
				int res = -1;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"UPDATE reservas 
						SET inmueble_id=@inmueble, inquilino_id=@inquilino, usuario_creador_id=@usuario_creador, usuario_cancelador_id=@usuario_cancelador, fecha_desde=@fecha_desde, fecha_hasta=@fecha_hasta, cancelada=@cancelada, fecha_creacion=@fecha_creacion, fecha_cancelacion=@fecha_cancelacion
						WHERE id = @id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@inmueble", p.Inmueble?.Id);
						command.Parameters.AddWithValue("@inquilino", p.Inquilino?.Id);
						command.Parameters.AddWithValue("@usuario_creador", p.UsuarioCreador?.Id);
						command.Parameters.AddWithValue("@usuario_cancelador", p.UsuarioCancelador?.Id);
						command.Parameters.AddWithValue("@fecha_desde", p.FechaDesde);
						command.Parameters.AddWithValue("@fecha_hasta", p.FechaHasta);
						command.Parameters.AddWithValue("@cancelada", !p.Activo);
						command.Parameters.AddWithValue("@fecha_creacion", p.FechaCreacion);
						command.Parameters.AddWithValue("@fecha_cancelacion", p.FechaCancelacion);
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
				Console.WriteLine($"Error RepositorioReserva - Modificacion: {ex.Message}");
				throw;
		   }
		}

		public Reserva? ObtenerPorId(int id)
		{
			try
			{
				Reserva? p = null;
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT id, inmueble_id, inquilino_id, usuario_creador_id, usuario_cancelador_id, fecha_desde, fecha_hasta, cancelada, fecha_creacion, fecha_cancelacion 
					FROM reservas
					WHERE id=@id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							p = MapearReserva(reader);
						}
						connection.Close();
					}
				}
				return p;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error RepositorioReserva - ObtenerPorId: {ex.Message}");
				throw;
			}
		}

		public IEnumerable<Reserva> ObtenerTodas()
		{
			try
			{
				List<Reserva> reservas = new List<Reserva>();

				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT 
							id,
							inmueble_id,
							inquilino_id,
							usuario_creador_id,
							usuario_cancelador_id,
							fecha_desde,
							fecha_hasta,
							cancelada,
							fecha_creacion,
							fecha_cancelacion
						FROM reservas
						ORDER BY id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
							reservas.Add(MapearReserva(reader));
						}
						connection.Close();
					}
				}
				
				return reservas;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error RepositorioBase - ObtenerTodas: {ex.Message}");
				throw;
			}
		}

		public IEnumerable<Reserva> ObtenerPorFecha(DateOnly fecha)
		{
			List<Reserva> reservas = new List<Reserva>();
			
			try
			{
				using (MySqlConnection connection = new MySqlConnection(connectionString))
				{
					string sql = @"SELECT 
						id,
						inmueble_id,
						inquilino_id,
						usuario_creador_id,
						usuario_cancelador_id,
						fecha_desde,
						fecha_hasta,
						cancelada,
						fecha_creacion,
						fecha_cancelacion
					FROM reservas
					WHERE @fecha_ingresada BETWEEN fecha_desde AND fecha_hasta
					ORDER BY id";
					using (MySqlCommand command = new MySqlCommand(sql, connection))
					{
						command.Parameters.Add("@fecha_ingresada", MySqlDbType.Date).Value = fecha;
						command.CommandType = CommandType.Text;
						connection.Open();
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
							reservas.Add(MapearReserva(reader));
						}
						connection.Close();
					}
				}
				return reservas;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error RepositorioReserva - ObtenerPorFecha: {ex.Message}");
				throw;
			}
		}

		public IEnumerable<Reserva> ObtenerPorInmueble(int id)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Reserva> ObtenerPorInquilino(int id)
		{
			throw new NotImplementedException();
		}

		private Reserva MapearReserva(MySqlDataReader reader)
		{
			return new Reserva
			{
				/* Los marcados como "a checar" usan el codigo ya hecho en las clases pertinentes 
				 * El problema es que en cada uno de ellos, se realiza otro query, por lo que
				 * mapear una reserva generaria 4 queries. Por lo que es exponencial y quiza 
				 * notable cuando se trata de "ObtenerTodas()" por ejemplo.

				 * Considerar cambiarlo para que funcione similar a RepositorioInmueble.cs
				 */
				Id = reader.GetInt32("id"),
				Inmueble = Inmuebles.ObtenerPorId(reader.GetInt32("inmueble_id")), 				//a chequear
				Inquilino = Inquilinos.ObtenerPorId(reader.GetInt32("inquilino_id")), 			//a chequear
				UsuarioCreador = Usuarios.ObtenerPorId(reader.GetInt32("usuario_creador_id")), 	//a chequear
				UsuarioCancelador = reader.IsDBNull(reader.GetInt32("usuario_cancelador_id"))	//a chequear
					? new Usuario {}
					: Usuarios.ObtenerPorId(reader.GetInt32("usuario_cancelador_id")),
				FechaDesde = reader.GetDateOnly("fecha_desde"),
				FechaHasta = reader.GetDateOnly("fecha_hasta"),
				Activo = !reader.GetBoolean("cancelada"),
				FechaCreacion = reader.GetDateTime("fecha_creacion"),
				FechaCancelacion = reader.GetDateTime("fecha_cancelacion")
			};
		}
	}
}
