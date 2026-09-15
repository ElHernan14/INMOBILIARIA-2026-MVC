using INMOBILIARIA.Models;

namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
    }
}