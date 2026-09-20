using INMOBILIARIA.Models.Interfaces;
using MySqlConnector;
using System.Data;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            try
            {
                int res = -1;

                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = @"
                    INSERT INTO pagos
                    (
                        reserva_id,
                        usuario_creador_id,
                        usuario_cancelador_id,
                        concepto,
                        fecha,
                        importe,
                        anulado,
                        fecha_creacion,
                        fecha_cancelacion
                    )
                    VALUES
                    (
                        @reserva_id,
                        @usuario_creador_id,
                        @usuario_cancelador_id,
                        @concepto,
                        @fecha,
                        @importe,
                        @anulado,
                        @fecha_creacion,
                        @fecha_cancelacion
                    );

                    SELECT LAST_INSERT_ID();";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@reserva_id", pago.Reserva!.Id);
                command.Parameters.AddWithValue("@usuario_creador_id", pago.UsuarioCreador!.Id);
                command.Parameters.AddWithValue(
                    "@usuario_cancelador_id",
                    pago.UsuarioCancelador?.Id ?? (object)DBNull.Value
                );
                command.Parameters.AddWithValue("@concepto", pago.Concepto);
                command.Parameters.AddWithValue("@fecha", pago.Fecha);
                command.Parameters.AddWithValue("@importe", pago.Importe);
                command.Parameters.AddWithValue("@anulado", pago.Anulado);
                command.Parameters.AddWithValue("@fecha_creacion", pago.FechaCreacion);
                command.Parameters.AddWithValue(
                    "@fecha_cancelacion",
                    pago.FechaCancelacion ?? (object)DBNull.Value
                );

                command.CommandType = CommandType.Text;

                connection.Open();

                res = Convert.ToInt32(command.ExecuteScalar());

                pago.Id = res;

                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - Alta: {ex.Message}");
                throw;
            }
        }

        public int Baja(int id, int usuarioCanceladorId)
        {
            try
            {
                using MySqlConnection connection =
                    new MySqlConnection(connectionString);

                string sql = @"
                    UPDATE pagos
                    SET
                        anulado = TRUE,
                        usuario_cancelador_id = @usuario_cancelador_id,
                        fecha_cancelacion = @fecha_cancelacion
                    WHERE id = @id
                    AND anulado = FALSE";

                using MySqlCommand command =
                    new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id", id);

                command.Parameters.AddWithValue(
                    "@usuario_cancelador_id",
                    usuarioCanceladorId);

                command.Parameters.AddWithValue(
                    "@fecha_cancelacion",
                    DateTime.Now);

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - Baja: {ex.Message}");
                throw;
            }
        }

        public int Baja(int id)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = @"
                    UPDATE pagos
                    SET
                        anulado = TRUE,
                        fecha_cancelacion = @fecha_cancelacion
                    WHERE id = @id
                      AND anulado = FALSE";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@fecha_cancelacion", DateTime.Now);

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - Baja: {ex.Message}");
                throw;
            }
        }

        public int Modificacion(Pago pago)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = @"
                    UPDATE pagos
                    SET
                        concepto = @concepto
                    WHERE id = @id
                      AND anulado = FALSE";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@concepto", pago.Concepto);
                command.Parameters.AddWithValue("@id", pago.Id);

                command.CommandType = CommandType.Text;

                connection.Open();

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - Modificacion: {ex.Message}");
                throw;
            }
        }

        public Pago? ObtenerPorId(int id)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = @"
                    SELECT
                        p.id,
                        p.reserva_id,
                        p.usuario_creador_id,
                        p.usuario_cancelador_id,
                        p.concepto,
                        p.fecha,
                        p.importe,
                        p.anulado,
                        p.fecha_creacion,
                        p.fecha_cancelacion,

                        r.fecha_desde,
                        r.fecha_hasta,
                        r.cancelada,

                        i.id AS inmueble_id,
                        i.direccion AS inmueble_direccion,

                        t.id AS tipo_id,
                        t.nombre AS tipo_nombre,

                        iq.id AS inquilino_id,
                        iq.nombre AS inquilino_nombre,
                        iq.apellido AS inquilino_apellido,
                        iq.dni AS inquilino_dni,

                        uc.id AS creador_id,
                        uc.nombre AS creador_nombre,
                        uc.apellido AS creador_apellido,

                        ux.id AS cancelador_id,
                        ux.nombre AS cancelador_nombre,
                        ux.apellido AS cancelador_apellido

                    FROM pagos p

                    INNER JOIN reservas r
                        ON p.reserva_id = r.id

                    INNER JOIN inmuebles i
                        ON r.inmueble_id = i.id

                    INNER JOIN tipos_inmueble t
                        ON i.tipo_inmueble_id = t.id

                    INNER JOIN inquilinos iq
                        ON r.inquilino_id = iq.id

                    INNER JOIN usuarios uc
                        ON p.usuario_creador_id = uc.id

                    LEFT JOIN usuarios ux
                        ON p.usuario_cancelador_id = ux.id

                    WHERE p.id = @id";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                connection.Open();

                using var reader = command.ExecuteReader();

                if (reader.Read())
                    return MapearPago(reader);

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - ObtenerPorId: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Pago> ObtenerTodas()
        {
            return ObtenerTodas(1, int.MaxValue).Resultados;
        }

        public PagedResults<Pago> ObtenerTodas(
            int page = 1,
            int limit = 10,
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null)
        {
            try
            {
                if (page < 1)
                    page = 1;

                if (limit < 1)
                    limit = 10;

                string filtros = ConstruirFiltros(
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                PagedResults<Pago> resultados = new()
                {
                    TotalResults = Contar(
                        fechaDesde,
                        fechaHasta,
                        termino,
                        anulado),
                    CurrentPage = page,
                    PageSize = limit
                };

                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = $@"
                    SELECT
                        p.id,
                        p.reserva_id,
                        p.usuario_creador_id,
                        p.usuario_cancelador_id,
                        p.concepto,
                        p.fecha,
                        p.importe,
                        p.anulado,
                        p.fecha_creacion,
                        p.fecha_cancelacion,

                        r.fecha_desde,
                        r.fecha_hasta,
                        r.cancelada,

                        i.id AS inmueble_id,
                        i.direccion AS inmueble_direccion,

                        t.id AS tipo_id,
                        t.nombre AS tipo_nombre,

                        iq.id AS inquilino_id,
                        iq.nombre AS inquilino_nombre,
                        iq.apellido AS inquilino_apellido,
                        iq.dni AS inquilino_dni,

                        uc.id AS creador_id,
                        uc.nombre AS creador_nombre,
                        uc.apellido AS creador_apellido,

                        ux.id AS cancelador_id,
                        ux.nombre AS cancelador_nombre,
                        ux.apellido AS cancelador_apellido

                    FROM pagos p

                    INNER JOIN reservas r
                        ON p.reserva_id = r.id

                    INNER JOIN inmuebles i
                        ON r.inmueble_id = i.id

                    INNER JOIN tipos_inmueble t
                        ON i.tipo_inmueble_id = t.id

                    INNER JOIN inquilinos iq
                        ON r.inquilino_id = iq.id

                    INNER JOIN usuarios uc
                        ON p.usuario_creador_id = uc.id

                    LEFT JOIN usuarios ux
                        ON p.usuario_cancelador_id = ux.id

                    {filtros}

                    ORDER BY p.fecha DESC, p.id DESC

                    LIMIT @limit OFFSET @offset";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                AgregarParametrosFiltros(
                    command,
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                command.Parameters.AddWithValue("@limit", limit);
                command.Parameters.AddWithValue(
                    "@offset",
                    (page - 1) * limit);

                connection.Open();

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    resultados.Resultados.Add(MapearPago(reader));
                }

                return resultados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - ObtenerTodas: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Reserva> ObtenerReservasDisponibles()
        {
            try
            {
                List<Reserva> reservas = new();

                using MySqlConnection connection = new MySqlConnection(connectionString);

                string sql = @"
                    SELECT
                        r.id,
                        r.fecha_desde,
                        r.fecha_hasta,
                        r.cancelada,

                        i.id AS inmueble_id,
                        i.direccion AS inmueble_direccion,

                        t.id AS tipo_id,
                        t.nombre AS tipo_nombre,

                        iq.id AS inquilino_id,
                        iq.nombre AS inquilino_nombre,
                        iq.apellido AS inquilino_apellido,
                        iq.dni AS inquilino_dni

                    FROM reservas r

                    INNER JOIN inmuebles i
                        ON r.inmueble_id = i.id

                    INNER JOIN tipos_inmueble t
                        ON i.tipo_inmueble_id = t.id

                    INNER JOIN inquilinos iq
                        ON r.inquilino_id = iq.id

                    WHERE r.cancelada = FALSE
                      AND i.activo = TRUE

                    ORDER BY r.fecha_desde DESC, r.id DESC";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                connection.Open();

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    reservas.Add(new Reserva
                    {
                        Id = reader.GetInt32("id"),

                        FechaDesde = reader.GetDateOnly("fecha_desde"),
                        FechaHasta = reader.GetDateOnly("fecha_hasta"),

                        Activo = !reader.GetBoolean("cancelada"),

                        Inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("inmueble_id"),

                            Direccion = reader.GetString("inmueble_direccion"),

                            TipoInmueble = new TipoInmueble
                            {
                                Id = reader.GetInt32("tipo_id"),
                                Nombre = reader.GetString("tipo_nombre")
                            }
                        },

                        Inquilino = new Inquilino
                        {
                            Id = reader.GetInt32("inquilino_id"),
                            Nombre = reader.GetString("inquilino_nombre"),
                            Apellido = reader.GetString("inquilino_apellido"),
                            Dni = reader.GetString("inquilino_dni")
                        }
                    });
                }

                return reservas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error RepositorioPago - ObtenerReservasDisponibles: {ex.Message}");

                throw;
            }
        }

        public ResumenPagos ObtenerResumen(
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);

                string filtros = ConstruirFiltros(
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                string sql = $@"
                    SELECT
                        COUNT(*) AS total_pagos,
                        COALESCE(
                            SUM(CASE WHEN p.anulado = FALSE THEN 1 ELSE 0 END),
                            0
                        ) AS pagos_activos,
                        COALESCE(
                            SUM(CASE WHEN p.anulado = TRUE THEN 1 ELSE 0 END),
                            0
                        ) AS pagos_anulados,
                        COALESCE(SUM(p.importe), 0) AS total_importe,
                        COALESCE(
                            SUM(CASE WHEN p.anulado = FALSE THEN p.importe ELSE 0 END),
                            0
                        ) AS importe_activo,
                        COALESCE(
                            SUM(CASE WHEN p.anulado = TRUE THEN p.importe ELSE 0 END),
                            0
                        ) AS importe_anulado
                    FROM pagos p
                    INNER JOIN reservas r
                        ON p.reserva_id = r.id
                    INNER JOIN inmuebles i
                        ON r.inmueble_id = i.id
                    INNER JOIN tipos_inmueble t
                        ON i.tipo_inmueble_id = t.id
                    INNER JOIN inquilinos iq
                        ON r.inquilino_id = iq.id
                    {filtros}";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                AgregarParametrosFiltros(
                    command,
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                connection.Open();

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    return new ResumenPagos();

                return new ResumenPagos
                {
                    TotalPagos = Convert.ToInt32(reader["total_pagos"]),
                    PagosActivos = Convert.ToInt32(reader["pagos_activos"]),
                    PagosAnulados = Convert.ToInt32(reader["pagos_anulados"]),

                    TotalImporte = Convert.ToDecimal(reader["total_importe"]),
                    ImporteActivo = Convert.ToDecimal(reader["importe_activo"]),
                    ImporteAnulado = Convert.ToDecimal(reader["importe_anulado"])
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - ObtenerResumen: {ex.Message}");
                throw;
            }
        }

        public int Contar(
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(connectionString);

                string filtros = ConstruirFiltros(
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                string sql = $@"
                    SELECT COUNT(*)
                    FROM pagos p
                    INNER JOIN reservas r
                        ON p.reserva_id = r.id
                    INNER JOIN inmuebles i
                        ON r.inmueble_id = i.id
                    INNER JOIN tipos_inmueble t
                        ON i.tipo_inmueble_id = t.id
                    INNER JOIN inquilinos iq
                        ON r.inquilino_id = iq.id
                    {filtros}";

                using MySqlCommand command = new MySqlCommand(sql, connection);

                AgregarParametrosFiltros(
                    command,
                    fechaDesde,
                    fechaHasta,
                    termino,
                    anulado);

                connection.Open();

                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error RepositorioPago - Contar: {ex.Message}");
                throw;
            }
        }

        private string ConstruirFiltros(
            DateOnly? fechaDesde,
            DateOnly? fechaHasta,
            string? termino,
            bool? anulado)
        {
            List<string> condiciones = new();

            if (fechaDesde.HasValue)
                condiciones.Add("p.fecha >= @fecha_desde");

            if (fechaHasta.HasValue)
                condiciones.Add("p.fecha <= @fecha_hasta");

            if (!string.IsNullOrWhiteSpace(termino))
            {
                condiciones.Add(@"(
                    p.concepto LIKE @termino
                    OR i.direccion LIKE @termino
                    OR t.nombre LIKE @termino
                    OR CONCAT(iq.nombre, ' ', iq.apellido) LIKE @termino
                )");
            }

            if (anulado.HasValue)
                condiciones.Add("p.anulado = @anulado");

            if (condiciones.Count == 0)
                return string.Empty;

            return "WHERE " + string.Join(" AND ", condiciones);
        }

        private void AgregarParametrosFiltros(
            MySqlCommand command,
            DateOnly? fechaDesde,
            DateOnly? fechaHasta,
            string? termino,
            bool? anulado)
        {
            if (fechaDesde.HasValue)
                command.Parameters.AddWithValue("@fecha_desde", fechaDesde.Value);

            if (fechaHasta.HasValue)
                command.Parameters.AddWithValue("@fecha_hasta", fechaHasta.Value);

            if (!string.IsNullOrWhiteSpace(termino))
                command.Parameters.AddWithValue("@termino", $"%{termino.Trim()}%");

            if (anulado.HasValue)
                command.Parameters.AddWithValue("@anulado", anulado.Value);
        }

        private Pago MapearPago(MySqlDataReader reader)
        {
            Usuario? cancelador = null;

            if (!reader.IsDBNull(reader.GetOrdinal("cancelador_id")))
            {
                cancelador = new Usuario
                {
                    Id = reader.GetInt32("cancelador_id"),
                    Nombre = reader.GetString("cancelador_nombre"),
                    Apellido = reader.GetString("cancelador_apellido")
                };
            }

            return new Pago
            {
                Id = reader.GetInt32("id"),

                Concepto = reader.IsDBNull(reader.GetOrdinal("concepto"))
                    ? string.Empty
                    : reader.GetString("concepto"),

                Fecha = reader.GetDateOnly("fecha"),
                Importe = reader.GetDecimal("importe"),

                Anulado = reader.GetBoolean("anulado"),

                FechaCreacion = reader.GetDateTime("fecha_creacion"),

                FechaCancelacion =
                    reader.IsDBNull(reader.GetOrdinal("fecha_cancelacion"))
                        ? null
                        : reader.GetDateTime("fecha_cancelacion"),

                Reserva = new Reserva
                {
                    Id = reader.GetInt32("reserva_id"),

                    FechaDesde = reader.GetDateOnly("fecha_desde"),
                    FechaHasta = reader.GetDateOnly("fecha_hasta"),

                    Activo = !reader.GetBoolean("cancelada"),

                    Inmueble = new Inmueble
                    {
                        Id = reader.GetInt32("inmueble_id"),

                        Direccion =
                            reader.GetString("inmueble_direccion"),


                        TipoInmueble = new TipoInmueble
                        {
                            Id = reader.GetInt32("tipo_id"),
                            Nombre = reader.GetString("tipo_nombre")
                        }
                    },

                    Inquilino = new Inquilino
                    {
                        Id = reader.GetInt32("inquilino_id"),
                        Nombre = reader.GetString("inquilino_nombre"),
                        Apellido = reader.GetString("inquilino_apellido"),
                        Dni = reader.GetString("inquilino_dni")
                    }
                },

                UsuarioCreador = new Usuario
                {
                    Id = reader.GetInt32("creador_id"),
                    Nombre = reader.GetString("creador_nombre"),
                    Apellido = reader.GetString("creador_apellido")
                },

                UsuarioCancelador = cancelador
            };
        }
    }
}