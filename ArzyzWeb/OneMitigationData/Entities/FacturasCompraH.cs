using System;

namespace ArzyzWeb.OneMitigationData.Entities
{
    public class FacturasCompraH
    {
        public string ordenCompra { get; set; }
        public string idFactura { get; set; }
        public DateTime fecha { get; set; }
        public string fecha_text { get; set; }
        public string cuentaProveedor { get; set; }
        public string moneda { get; set; }
        public decimal impuestos { get; set; }
        public decimal montototal { get; set; }
        public string condicionPago { get; set; }
        public string empresa { get; set; }
        public string grupoProveedor { get; set; }
    }
}
