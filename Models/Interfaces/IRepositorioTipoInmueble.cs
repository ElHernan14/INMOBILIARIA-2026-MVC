namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioTipoInmueble
    {
        int Alta(TipoInmueble t);
        int Baja(int id);
        int Modificacion(TipoInmueble t);
        
    }
}