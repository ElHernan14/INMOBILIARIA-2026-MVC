using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {

        List<Inmueble> ObtenerTodos(bool activo = true, bool disponible = true, string? direccion = null, int limit = 10, int page = 1);

        int ContarTodos(bool activo = true, bool disponible = true, string? direccion = null);
    }
}