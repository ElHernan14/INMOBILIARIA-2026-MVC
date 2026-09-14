namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        //public int Alta(Usuario p);
        //public int Baja(int id);
        //public int Modificacion(Usuario p);
        //public Usuario? ObtenerPorId(int id);
        public Usuario? ObtenerPorEmail(String email);
    }
}