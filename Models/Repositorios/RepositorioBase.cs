namespace INMOBILIARIA.Models;
// ESTO ES PARA LA BD
public abstract class RepositorioBase
{
    protected readonly IConfiguration configuration;
    protected readonly string connectionString;

    protected RepositorioBase(IConfiguration configuration)
    {
        this.configuration = configuration;
        connectionString = configuration["ConnectionStrings:MySqlConnectionString"]!;
        // connectionString = "Server=localhost;Port=3307;User=root;Password=1234;Database=mi_base;SslMode=none";
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("La cadena de conexión 'ConnectionStrings:MySql' no está configurada.");
        }
    }
}
