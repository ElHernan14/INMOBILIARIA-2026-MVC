namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInquilino
    {
        int Alta(Inquilino i);
        int Baja(int id);
        int Modificacion(Inquilino i);

    }
}