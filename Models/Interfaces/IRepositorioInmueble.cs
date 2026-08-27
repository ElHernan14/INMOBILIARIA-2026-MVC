using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInmueble
    {
        int Alta(Inmueble inmueble);

        int Baja(int id);

        int Modificacion(Inmueble inmueble);

        Inmueble ObtenerPorId(int id);

        IEnumerable<Inmueble> ObtenerTodos();
    }
}