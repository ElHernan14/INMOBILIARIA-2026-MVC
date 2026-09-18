namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
    {
        //public abstract int Alta(Reserva p);
        //public abstract int Baja(int id);
        //public abstract int Modificacion(Reserva p);
        //public abstract Reserva? ObtenerPorId(int id);
        public abstract IEnumerable<Reserva> ObtenerTodas();
        public abstract PagedResults<Reserva> ObtenerTodas(int page = 1, int limit = 10);
        public abstract IEnumerable<Reserva> ObtenerPorInmueble(int id);
        public abstract IEnumerable<Reserva> ObtenerPorInmuebleFuturas(int id);
        public abstract IEnumerable<Reserva> ObtenerPorInquilino(int id);
        public abstract IEnumerable<Reserva> ObtenerPorFecha(DateOnly fecha);
    }
}