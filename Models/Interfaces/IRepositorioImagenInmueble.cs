using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioImagenInmueble
    {
        int Alta(ImagenInmueble imagen);

        IEnumerable<ImagenInmueble> ObtenerPorInmueble(
            int inmuebleId);

        ImagenInmueble? ObtenerPorId(
            int id);

        void EstablecerPortada(
            int id,
            int inmuebleId);

        int Eliminar(
            int id);
    }
}