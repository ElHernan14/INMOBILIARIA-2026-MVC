namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        List<Inquilino> ObtenerTodos(bool activo = true, string? nombre = null, string? apellido = null, string? dni = null, string? email = null, int limit = 10, int page = 1);

        int ContarTodos(bool activo = true, string? nombre = null, string? apellido = null, string? dni = null, string? email = null);
        
    }
}