namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioUsuario
    {
        public abstract int Alta(Usuario p);
        public abstract int Baja(int id);
        public abstract int Modificacion(Usuario p);
        public abstract Usuario? ObtenerPorId(int id);
    }
}