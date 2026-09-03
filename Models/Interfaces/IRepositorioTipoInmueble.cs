namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioTipoInmueble : IRepositorio<TipoInmueble>
    {
        // desarrollar con lo que falte
        List<TipoInmueble> ObtenerTodos();
    }
}