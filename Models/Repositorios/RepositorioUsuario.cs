using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration)
            : base(configuration)
        {
        }

        public int Alta(Usuario p)
        {
            try
            {
                const string sql = @"
                    INSERT INTO usuarios
                    (
                        nombre,
                        apellido,
                        dni,
                        email,
                        avatar,
                        password,
                        rol,
                        activo
                    )
                    VALUES
                    (
                        @nombre,
                        @apellido,
                        @dni,
                        @email,
                        @avatar,
                        @password,
                        @rol,
                        @activo
                    );

                    SELECT LAST_INSERT_ID();";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);

                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue(
                    "@avatar",
                    string.IsNullOrWhiteSpace(p.Avatar)
                        ? DBNull.Value
                        : p.Avatar);
                command.Parameters.AddWithValue("@password", p.Password);
                command.Parameters.AddWithValue("@rol", p.Rol.ToString());
                command.Parameters.AddWithValue("@activo", p.Activo);

                connection.Open();

                int id = Convert.ToInt32(command.ExecuteScalar());

                p.Id = id;

                return id;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error RepositorioUsuario - Alta: {ex.Message}");

                throw;
            }
        }

        public int Baja(int id)
        {
            try
            {
                const string sql = @"
                    UPDATE usuarios
                    SET activo = FALSE
                    WHERE id = @id;";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);

                command.CommandType = CommandType.Text;
                command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error RepositorioUsuario - Baja: {ex.Message}");

                throw;
            }
        }

        public int Modificacion(Usuario p)
        {
            try
            {
                const string sql = @"
                    UPDATE usuarios
                    SET
                        nombre = @nombre,
                        apellido = @apellido,
                        dni = @dni,
                        email = @email,
                        avatar = @avatar,
                        password = @password,
                        rol = @rol,
                        activo = @activo
                    WHERE id = @id;";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);

                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@id", p.Id);
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.Dni);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue(
                    "@avatar",
                    string.IsNullOrWhiteSpace(p.Avatar)
                        ? DBNull.Value
                        : p.Avatar);
                command.Parameters.AddWithValue("@password", p.Password);
                command.Parameters.AddWithValue("@rol", p.Rol.ToString());
                command.Parameters.AddWithValue("@activo", p.Activo);

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error RepositorioUsuario - Modificacion: {ex.Message}");

                throw;
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            try
            {
                const string sql = @"
                    SELECT
                        id,
                        nombre,
                        apellido,
                        dni,
                        email,
                        avatar,
                        password,
                        rol,
                        activo
                    FROM usuarios
                    WHERE id = @id;";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);

                command.CommandType = CommandType.Text;
                command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                connection.Open();

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return Mapear(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error RepositorioUsuario - ObtenerPorId: {ex.Message}");

                throw;
            }
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            try
            {
                const string sql = @"
                    SELECT
                        id,
                        nombre,
                        apellido,
                        dni,
                        email,
                        avatar,
                        password,
                        rol,
                        activo
                    FROM usuarios
                    WHERE LOWER(email) = LOWER(@email)
                    LIMIT 1;";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);

                command.CommandType = CommandType.Text;
                command.Parameters.Add("@email", MySqlDbType.VarChar)
                    .Value = email.Trim();

                connection.Open();

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return Mapear(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Error RepositorioUsuario - ObtenerPorEmail: {ex.Message}");

                throw;
            }
        }

        private Usuario Mapear(MySqlDataReader reader)
        {
            string rolTexto = reader.GetString("rol");

            if (!Enum.TryParse<RolUsuario>(
                    rolTexto,
                    ignoreCase: true,
                    out var rol))
            {
                throw new InvalidOperationException(
                    $"El rol '{rolTexto}' no es válido.");
            }

            return new Usuario
            {
                Id = reader.GetInt32("id"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Dni = reader.GetString("dni"),
                Email = reader.GetString("email"),
                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar"))
                    ? string.Empty
                    : reader.GetString("avatar"),
                Password = reader.GetString("password"),
                Rol = rol,
                Activo = reader.GetBoolean("activo")
            };
        }
    }
}