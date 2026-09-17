namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        IEnumerable<Pago> ObtenerTodas();

        PagedResults<Pago> ObtenerTodas(
            int page = 1,
            int limit = 10,
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null);

        IEnumerable<Reserva> ObtenerReservasDisponibles();

        ResumenPagos ObtenerResumen(
            DateOnly? fechaDesde = null,
            DateOnly? fechaHasta = null,
            string? termino = null,
            bool? anulado = null);

        int Baja(int id, int usuarioCanceladorId);
    }
}