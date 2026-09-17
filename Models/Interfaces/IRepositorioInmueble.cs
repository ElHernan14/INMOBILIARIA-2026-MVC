using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IEnumerable<Inmueble> ObtenerTodos();

        IEnumerable<Inmueble> ObtenerTodos(
            string termino,
            bool? disponible,
            bool? activo,
            int limit,
            int page);

        int Contar(
            string termino,
            bool? disponible,
            bool? activo);
    }
}