namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioReserva
    {
        public abstract int Alta(Reserva p);
        public abstract int Baja(int id);
        public abstract int Modificacion(Reserva p);
        public abstract Reserva? ObtenerPorId(int id);
        public abstract IEnumerable<Reserva> ObtenerTodas();
        public abstract IEnumerable<Reserva> ObtenerPorInmueble(int id);
        public abstract IEnumerable<Reserva> ObtenerPorInquilino(int id);
        public abstract IEnumerable<Reserva> ObtenerPorFecha(DateOnly fecha);
    }
}