namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioReserva
    {
        Usuario Alta(Reserva r);
        Usuario Baja(int id);
        Usuario Modificacion(Reserva r);
        
    }
}