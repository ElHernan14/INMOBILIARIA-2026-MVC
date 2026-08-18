namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioPago
    {
        Usuario Alta(Pago p);
        Usuario Baja(int id);
        Usuario Modificacion(Pago p);
        
    }
}