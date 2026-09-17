using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
        IList<Usuario> ObtenerTodos(int limit, int page);
        int ObtenerCantidad();
    }
}