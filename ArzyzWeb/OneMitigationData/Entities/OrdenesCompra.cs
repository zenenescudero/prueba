using System;

namespace ArzyzWeb.OneMitigationData.Entities
{
    public class OrdenesCompra
    {
        public string OrdenCompra { get; set; }
        public string CuentaProveedor { get; set; }
        public string nombreProveedor { get; set; }
        public DateTime fecha { get; set; }
        public string fecha_text { get; set; }
        public string moneda { get; set; }
        public string articulo { get; set; }
        public string nombreArticulo { get; set; }
        public string condicionPago { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public long numLinea { get; set; }
        public decimal impuestos { get; set; }
        public string Empresa { get; set; }
        public decimal monto { get; set; }
        public int? tipoProveedor { get; set; }
    }
}
