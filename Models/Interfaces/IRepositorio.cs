namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorio<T>
    {
        int Alta(T entidad);

        int Baja(int id);

        int Modificacion(T entidad);

        // Devuelve null si no encuentra la entidad.
        T? ObtenerPorId(int id);
    }
}