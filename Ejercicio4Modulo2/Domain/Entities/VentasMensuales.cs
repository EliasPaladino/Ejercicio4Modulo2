
namespace Ejercicio4Modulo2.Domain.Entities
{
    public partial class VentasMensuales
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string CodVendedor { get; set; }
        public decimal Venta { get; set; }
        public bool VentaEmpresaGrande { get; set; }
    }
}