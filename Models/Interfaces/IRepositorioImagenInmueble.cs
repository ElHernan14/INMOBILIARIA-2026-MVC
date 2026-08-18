namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioImagenInmueble
    {
        int Alta(ImagenInmueble i);
        int Baja(int id);
        int Modificacion(Inquilino i);

    }
}