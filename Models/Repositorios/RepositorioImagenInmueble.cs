using INMOBILIARIA.Models;
using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioImagenInmueble : RepositorioBase, IRepositorioImagenInmueble
    {
        public RepositorioImagenInmueble(
            IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(ImagenInmueble imagen)
        {
            if (imagen.Inmueble == null ||
                imagen.Inmueble.Id <= 0)
            {
                throw new ArgumentException(
                    "La imagen debe estar asociada a un inmueble válido.");
            }

            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            string sql = @"
                INSERT INTO imagenes_inmueble
                (
                    path,
                    es_portada,
                    inmueble_id
                )
                VALUES
                (
                    @path,
                    @es_portada,
                    @inmueble_id
                );

                SELECT LAST_INSERT_ID();";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@path",
                imagen.Path);

            command.Parameters.AddWithValue(
                "@es_portada",
                imagen.EsPortada);

            command.Parameters.AddWithValue(
                "@inmueble_id",
                imagen.Inmueble.Id);

            return Convert.ToInt32(
                command.ExecuteScalar());
        }

        public IEnumerable<ImagenInmueble> ObtenerPorInmueble(
            int inmuebleId)
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            string sql = @"
                SELECT
                    id,
                    path,
                    es_portada,
                    inmueble_id
                FROM imagenes_inmueble
                WHERE inmueble_id = @inmueble_id
                ORDER BY es_portada DESC, id ASC;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@inmueble_id",
                inmuebleId);

            using var reader =
                command.ExecuteReader();

            List<ImagenInmueble> imagenes = new();

            while (reader.Read())
            {
                imagenes.Add(
                    new ImagenInmueble
                    {
                        Id = reader.GetInt32("id"),
                        Path = reader.GetString("path"),
                        EsPortada = reader.GetBoolean("es_portada"),
                        Inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("inmueble_id")
                        }
                    });
            }

            return imagenes;
        }

        public ImagenInmueble? ObtenerPorId(int id)
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            string sql = @"
                SELECT
                    ii.id,
                    ii.path,
                    ii.es_portada,
                    i.id AS inmueble_id
                FROM imagenes_inmueble ii
                INNER JOIN inmuebles i
                    ON i.id = ii.inmueble_id
                WHERE ii.id = @id;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                id);

            using var reader =
                command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new ImagenInmueble
            {
                Id = reader.GetInt32("id"),
                Path = reader.GetString("path"),
                EsPortada = reader.GetBoolean("es_portada"),
                Inmueble = new Inmueble
                {
                    Id = reader.GetInt32("inmueble_id")
                }
            };
        }

        public void EstablecerPortada(
            int id,
            int inmuebleId)
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            using var transaction =
                connection.BeginTransaction();

            string quitarPortadasSQL = @"
                UPDATE imagenes_inmueble
                SET es_portada = 0
                WHERE inmueble_id = @inmueble_id;";

            using var quitarPortadasCommand =
                new MySqlCommand(
                    quitarPortadasSQL,
                    connection,
                    transaction);

            quitarPortadasCommand.Parameters.AddWithValue(
                "@inmueble_id",
                inmuebleId);

            quitarPortadasCommand.ExecuteNonQuery();

            string establecerPortadaSQL = @"
                UPDATE imagenes_inmueble
                SET es_portada = 1
                WHERE id = @id
                  AND inmueble_id = @inmueble_id;";

            using var establecerPortadaCommand =
                new MySqlCommand(
                    establecerPortadaSQL,
                    connection,
                    transaction);

            establecerPortadaCommand.Parameters.AddWithValue(
                "@id",
                id);

            establecerPortadaCommand.Parameters.AddWithValue(
                "@inmueble_id",
                inmuebleId);

            establecerPortadaCommand.ExecuteNonQuery();

            transaction.Commit();
        }

        public int Eliminar(int id)
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            string sql = @"
                DELETE FROM imagenes_inmueble
                WHERE id = @id;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                id);

            return command.ExecuteNonQuery();
        }
    }
}