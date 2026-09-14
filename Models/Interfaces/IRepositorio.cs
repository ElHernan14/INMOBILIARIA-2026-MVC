namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorio<T>
    {
        abstract int Alta(T p);
		abstract int Baja(int id);
		abstract int Modificacion(T p);

		// IList<T> ObtenerTodos();

		/* ObtenerPorId regresa null si no encuentra la fila
		 * en la base de datos. No se si es la mejor solución. 
		 */
		abstract T? ObtenerPorId(int id);
    }
}