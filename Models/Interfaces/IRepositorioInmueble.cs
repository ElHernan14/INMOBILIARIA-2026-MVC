namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInmueble
    {
        int Alta(Inmueble i);
        int Baja(int id);
        int Modificacion(Inquilino i);

    }
}