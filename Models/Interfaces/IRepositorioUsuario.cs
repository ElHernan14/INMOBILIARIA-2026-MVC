namespace INMOBILIARIA.Models.Interfaces
{
    public interface IRepositorioUsuario
    {
        Usuario Alta(Usuario u);
        Usuario Baja(int id);
        Usuario Modificacion(Usuario u);
        
    }
}