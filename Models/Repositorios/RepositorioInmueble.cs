using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Inmueble inmueble)
        {
            try
            {
                int res = -1;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO inmuebles 
                        (propietario_id, tipo_inmueble_id, direccion, cordenadas, cupo, precio_dia, porcentaje_reserva, disponible, activo)
                        VALUES 
                        (@propietario_id, @tipo_inmueble_id, @direccion, @cordenadas, @cupo, @precio_dia, @porcentaje_reserva, @disponible, @activo);
                        SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@propietario_id", inmueble.Propietario!.Id);
                        command.Parameters.AddWithValue("@tipo_inmueble_id", inmueble.TipoInmueble!.Id);
                        command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                        command.Parameters.AddWithValue("@cordenadas", inmueble.Cordenadas);
                        command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                        command.Parameters.AddWithValue("@precio_dia", inmueble.PrecioDia);
                        command.Parameters.AddWithValue("@porcentaje_reserva", inmueble.PorcentajeReserva);
                        command.Parameters.AddWithValue("@disponible", inmueble.Disponible);
                        command.Parameters.AddWithValue("@activo", inmueble.Activo);

                        connection.Open();

                        res = Convert.ToInt32(command.ExecuteScalar());

                        inmueble.Id = res;

                        connection.Close();
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioInmueble - Alta: {ex.Message}");
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
                    string sql = "UPDATE inmuebles SET activo=0 WHERE id = @id";

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
                Console.WriteLine($"Error RepositorioInmueble - Baja: {ex.Message}");
                throw;
            }
        }

        public int Modificacion(Inmueble inmueble)
        {
            try
            {
                int res = -1;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"UPDATE inmuebles
                        SET propietario_id=@propietario_id,
                            tipo_inmueble_id=@tipo_inmueble_id,
                            direccion=@direccion,
                            cordenadas=@cordenadas,
                            cupo=@cupo,
                            precio_dia=@precio_dia,
                            porcentaje_reserva=@porcentaje_reserva,
                            disponible=@disponible,
                            activo=@activo
                        WHERE id=@id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;

                        command.Parameters.AddWithValue("@propietario_id", inmueble.Propietario!.Id);
                        command.Parameters.AddWithValue("@tipo_inmueble_id", inmueble.TipoInmueble!.Id);
                        command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                        command.Parameters.AddWithValue("@cordenadas", inmueble.Cordenadas);
                        command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                        command.Parameters.AddWithValue("@precio_dia", inmueble.PrecioDia);
                        command.Parameters.AddWithValue("@porcentaje_reserva", inmueble.PorcentajeReserva);
                        command.Parameters.AddWithValue("@disponible", inmueble.Disponible);
                        command.Parameters.AddWithValue("@activo", inmueble.Activo);
                        command.Parameters.AddWithValue("@id", inmueble.Id);

                        connection.Open();

                        res = command.ExecuteNonQuery();

                        connection.Close();
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioInmueble - Modificacion: {ex.Message}");
                throw;
            }
        }

        public Inmueble ObtenerPorId(int id)
        {
            try
            {
                Inmueble inmueble = null;

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"SELECT 
                            i.id,
                            i.propietario_id,
                            i.tipo_inmueble_id,
                            i.direccion,
                            i.cordenadas,
                            i.cupo,
                            i.precio_dia,
                            i.porcentaje_reserva,
                            i.disponible,
                            i.activo,

                            p.id AS propietario_id,
                            p.nombre AS propietario_nombre,
                            p.apellido AS propietario_apellido,
                            p.dni AS propietario_dni,
                            p.email AS propietario_email,
                            p.activo AS propietario_activo,

                            t.id AS tipo_id,
                            t.nombre AS tipo_nombre,
                            t.descripcion AS tipo_descripcion

                        FROM inmuebles i
                        INNER JOIN propietarios p ON i.propietario_id = p.id
                        INNER JOIN tipos_inmueble t ON i.tipo_inmueble_id = t.id
                        WHERE i.id = @id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                        command.CommandType = CommandType.Text;

                        connection.Open();

                        var reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            inmueble = MapearInmueble(reader);
                        }

                        connection.Close();
                    }
                }

                return inmueble;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioInmueble - ObtenerPorId: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Inmueble> ObtenerTodos()
        {
            try
            {
                List<Inmueble> inmuebles = new List<Inmueble>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string sql = @"SELECT 
                            i.id,
                            i.propietario_id,
                            i.tipo_inmueble_id,
                            i.direccion,
                            i.cordenadas,
                            i.cupo,
                            i.precio_dia,
                            i.porcentaje_reserva,
                            i.disponible,
                            i.activo,

                            p.id AS propietario_id,
                            p.nombre AS propietario_nombre,
                            p.apellido AS propietario_apellido,
                            p.dni AS propietario_dni,
                            p.email AS propietario_email,
                            p.activo AS propietario_activo,

                            t.id AS tipo_id,
                            t.nombre AS tipo_nombre,
                            t.descripcion AS tipo_descripcion

                        FROM inmuebles i
                        INNER JOIN propietarios p ON i.propietario_id = p.id
                        INNER JOIN tipos_inmueble t ON i.tipo_inmueble_id = t.id
                        ORDER BY i.id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;

                        connection.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            inmuebles.Add(MapearInmueble(reader));
                        }

                        connection.Close();
                    }
                }

                return inmuebles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioInmueble - ObtenerTodos: {ex.Message}");
                throw;
            }
        }

        private Inmueble MapearInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                Id = reader.GetInt32("id"),

                Propietario = new Propietario
                {
                    Id = reader.GetInt32("propietario_id"),
                    Nombre = reader.GetString("propietario_nombre"),
                    Apellido = reader.GetString("propietario_apellido"),
                    Dni = reader.GetString("propietario_dni"),
                    Email = reader.GetString("propietario_email"),
                    Activo = reader.GetBoolean("propietario_activo")
                },

                TipoInmueble = new TipoInmueble
                {
                    Id = reader.GetInt32("tipo_id"),
                    Nombre = reader.GetString("tipo_nombre"),
                    Descripcion = reader.GetString("tipo_descripcion")
                },

                Direccion = reader.GetString("direccion"),
                Cordenadas = reader.IsDBNull(reader.GetOrdinal("cordenadas"))
                    ? string.Empty
                    : reader.GetString("cordenadas"),
                Cupo = reader.GetInt32("cupo"),
                PrecioDia = reader.GetDecimal("precio_dia"),
                PorcentajeReserva = reader.GetDecimal("porcentaje_reserva"),
                Disponible = reader.GetBoolean("disponible"),
                Activo = reader.GetBoolean("activo")
            };
        }
    }
}