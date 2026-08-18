using INMOBILIARIA.Models.Interfaces;

namespace INMOBILIARIA.Models.Repositorios
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {}

        public int Alta(Propietario p)
        {
            return 1;
        }
        
        public int Baja(int id)
        {
            return 1;
        }

        public int Modificacion(Propietario p)
        {
            return 1;
        }
    }
}