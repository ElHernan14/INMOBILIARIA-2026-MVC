namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioPropietario
    {
        int Alta(Propietario p);
        int Baja(int id);
        int Modificacion(Propietario p);

    }
}