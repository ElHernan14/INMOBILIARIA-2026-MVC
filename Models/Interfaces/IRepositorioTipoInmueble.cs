namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
    {
        // desarrollar con lo que falte
        abstract List<TipoInmueble> ObtenerTodos();
        abstract PagedResults<TipoInmueble> ObtenerTodos(int page, int limit);
        abstract int Contar();
    }
}