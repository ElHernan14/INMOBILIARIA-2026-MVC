namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        List<Propietario> ObtenerTodos(int activo, string nombreApellido, int limit, int page);
        int ContarTodos(int activo, string nombreApellido);
    }
}