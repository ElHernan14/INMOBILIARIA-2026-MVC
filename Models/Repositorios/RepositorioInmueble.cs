using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {

        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                INSERT INTO inmuebles
                (
                    propietario_id,
                    tipo_inmueble_id,
                    direccion,
                    latitud,
                    longitud,
                    cupo,
                    porcentaje_reserva,
                    disponible,
                    activo
                )
                VALUES
                (
                    @propietario_id,
                    @tipo_inmueble_id,
                    @direccion,
                    @latitud,
                    @longitud,
                    @cupo,
                    @porcentaje_reserva,
                    @disponible,
                    @activo
                );

                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@propietario_id",
                inmueble.Propietario?.Id ?? 0);

            command.Parameters.AddWithValue(
                "@tipo_inmueble_id",
                inmueble.TipoInmueble?.Id ?? 0);

            command.Parameters.AddWithValue(
                "@direccion",
                inmueble.Direccion);

            command.Parameters.AddWithValue(
                "@latitud",
                inmueble.Latitud.HasValue
                    ? inmueble.Latitud.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@longitud",
                inmueble.Longitud.HasValue
                    ? inmueble.Longitud.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@cupo",
                inmueble.Cupo);

            command.Parameters.AddWithValue(
                "@porcentaje_reserva",
                inmueble.PorcentajeReserva);

            command.Parameters.AddWithValue(
                "@disponible",
                inmueble.Disponible);

            command.Parameters.AddWithValue(
                "@activo",
                inmueble.Activo);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public int Baja(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                UPDATE inmuebles
                SET activo = 0
                WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                id);

            return command.ExecuteNonQuery();
        }

        public int Modificacion(Inmueble inmueble)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                UPDATE inmuebles
                SET
                    propietario_id = @propietario_id,
                    tipo_inmueble_id = @tipo_inmueble_id,
                    direccion = @direccion,
                    latitud = @latitud,
                    longitud = @longitud,
                    cupo = @cupo,
                    porcentaje_reserva = @porcentaje_reserva,
                    disponible = @disponible,
                    activo = @activo
                WHERE id = @id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                inmueble.Id);

            command.Parameters.AddWithValue(
                "@propietario_id",
                inmueble.Propietario?.Id ?? 0);

            command.Parameters.AddWithValue(
                "@tipo_inmueble_id",
                inmueble.TipoInmueble?.Id ?? 0);

            command.Parameters.AddWithValue(
                "@direccion",
                inmueble.Direccion);

            command.Parameters.AddWithValue(
                "@latitud",
                inmueble.Latitud.HasValue
                    ? inmueble.Latitud.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@longitud",
                inmueble.Longitud.HasValue
                    ? inmueble.Longitud.Value
                    : DBNull.Value);

            command.Parameters.AddWithValue(
                "@cupo",
                inmueble.Cupo);

            command.Parameters.AddWithValue(
                "@porcentaje_reserva",
                inmueble.PorcentajeReserva);

            command.Parameters.AddWithValue(
                "@disponible",
                inmueble.Disponible);

            command.Parameters.AddWithValue(
                "@activo",
                inmueble.Activo);

            return command.ExecuteNonQuery();
        }

        public Inmueble? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT
                    i.id,
                    i.propietario_id,
                    i.tipo_inmueble_id,
                    i.direccion,
                    i.latitud,
                    i.longitud,
                    i.cupo,
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
                    t.descripcion AS tipo_descripcion,
                    t.activo AS tipo_activo

                FROM inmuebles i

                INNER JOIN propietarios p
                    ON i.propietario_id = p.id

                INNER JOIN tipos_inmueble t
                    ON i.tipo_inmueble_id = t.id

                WHERE i.id = @id;";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                id);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapearInmueble(reader);
        }

        public IEnumerable<Inmueble> ObtenerTodos()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = @"
                SELECT
                    i.id,
                    i.propietario_id,
                    i.tipo_inmueble_id,
                    i.direccion,
                    i.latitud,
                    i.longitud,
                    i.cupo,
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
                    t.descripcion AS tipo_descripcion,
                    t.activo AS tipo_activo

                FROM inmuebles i

                INNER JOIN propietarios p
                    ON i.propietario_id = p.id

                INNER JOIN tipos_inmueble t
                    ON i.tipo_inmueble_id = t.id

                ORDER BY i.id DESC;";

            using var command = new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            List<Inmueble> inmuebles = new();

            while (reader.Read())
            {
                inmuebles.Add(
                    MapearInmueble(reader));
            }

            return inmuebles;
        }

       public IEnumerable<Inmueble> ObtenerTodos(
            string termino,
            bool? disponible,
            bool? activo,
            int limit = 10,
            int page = 1)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            int offset = (page - 1) * limit;

            List<string> filtrosSQL = new();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                filtrosSQL.Add(@"
                    (
                        i.direccion LIKE @termino
                        OR CONCAT(
                            p.nombre,
                            ' ',
                            p.apellido
                        ) LIKE @termino
                        OR t.nombre LIKE @termino
                    )");
            }

            if (disponible.HasValue)
            {
                filtrosSQL.Add(
                    "i.disponible = @disponible");
            }

            if (activo.HasValue)
            {
                filtrosSQL.Add(
                    "i.activo = @activo");
            }

            string whereSQL = filtrosSQL.Count > 0
                ? "WHERE " + string.Join(
                    " AND ",
                    filtrosSQL)
                : string.Empty;

            string sql = $@"
                SELECT
                    i.id,
                    i.propietario_id,
                    i.tipo_inmueble_id,
                    i.direccion,
                    i.latitud,
                    i.longitud,
                    i.cupo,
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
                    t.descripcion AS tipo_descripcion,
                    t.activo AS tipo_activo

                FROM inmuebles i

                INNER JOIN propietarios p
                    ON i.propietario_id = p.id

                INNER JOIN tipos_inmueble t
                    ON i.tipo_inmueble_id = t.id

                {whereSQL}

                ORDER BY i.id DESC

                LIMIT @limit
                OFFSET @offset;";

            using var command = new MySqlCommand(
                sql,
                connection);

            if (!string.IsNullOrWhiteSpace(termino))
            {
                command.Parameters.AddWithValue(
                    "@termino",
                    $"%{termino.Trim()}%");
            }

            if (disponible.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@disponible",
                    disponible.Value);
            }

            if (activo.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@activo",
                    activo.Value);
            }

            command.Parameters.AddWithValue(
                "@limit",
                limit);

            command.Parameters.AddWithValue(
                "@offset",
                offset);

            using var reader = command.ExecuteReader();

            List<Inmueble> inmuebles = new();

            while (reader.Read())
            {
                inmuebles.Add(
                    MapearInmueble(reader));
            }

            return inmuebles;
        }

        public int Contar(
            string termino,
            bool? disponible,
            bool? activo)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            List<string> filtrosSQL = new();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                filtrosSQL.Add(@"
                    (
                        i.direccion LIKE @termino
                        OR CONCAT(
                            p.nombre,
                            ' ',
                            p.apellido
                        ) LIKE @termino
                        OR t.nombre LIKE @termino
                    )");
            }

            if (disponible.HasValue)
            {
                filtrosSQL.Add(
                    "i.disponible = @disponible");
            }

            if (activo.HasValue)
            {
                filtrosSQL.Add(
                    "i.activo = @activo");
            }

            string whereSQL = filtrosSQL.Count > 0
                ? "WHERE " + string.Join(
                    " AND ",
                    filtrosSQL)
                : string.Empty;

            string sql = $@"
                SELECT COUNT(*)

                FROM inmuebles i

                INNER JOIN propietarios p
                    ON i.propietario_id = p.id

                INNER JOIN tipos_inmueble t
                    ON i.tipo_inmueble_id = t.id

                {whereSQL};";

            using var command = new MySqlCommand(
                sql,
                connection);

            if (!string.IsNullOrWhiteSpace(termino))
            {
                command.Parameters.AddWithValue(
                    "@termino",
                    $"%{termino.Trim()}%");
            }

            if (disponible.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@disponible",
                    disponible.Value);
            }

            if (activo.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@activo",
                    activo.Value);
            }

            return Convert.ToInt32(
                command.ExecuteScalar());
        }

        private Inmueble MapearInmueble(
            MySqlDataReader reader)
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
                    Descripcion = reader.GetString("tipo_descripcion"),
                    Activo = reader.GetBoolean("tipo_activo")
                },

                Direccion = reader.GetString("direccion"),

                Latitud = reader.IsDBNull(
                    reader.GetOrdinal("latitud"))
                    ? null
                    : reader.GetDecimal("latitud"),

                Longitud = reader.IsDBNull(
                    reader.GetOrdinal("longitud"))
                    ? null
                    : reader.GetDecimal("longitud"),

                Cupo = reader.GetInt32("cupo"),

                PorcentajeReserva =
                    reader.GetDecimal("porcentaje_reserva"),

                Disponible =
                    reader.GetBoolean("disponible"),

                Activo =
                    reader.GetBoolean("activo")
            };
        }
    }
}