namespace INMOBILIARIA.Models
{
    public class ResumenPagos
    {
        public int TotalPagos { get; set; }
        public int PagosActivos { get; set; }
        public int PagosAnulados { get; set; }

        public decimal TotalImporte { get; set; }
        public decimal ImporteActivo { get; set; }
        public decimal ImporteAnulado { get; set; }
    }
}