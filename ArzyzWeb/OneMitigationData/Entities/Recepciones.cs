using System;

namespace ArzyzWeb.OneMitigationData.Entities
{
    public class Recepciones
    {
        public string IdRecepcion { get; set; }
        public DateTime fechaRecepcion { get; set; }
        public string fecha_text { get; set; }
        public string ordenCompra { get; set; }
        public string articulo { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public string nombreArticulo { get; set; }
        public string configuracion { get; set; }
        public string lote { get; set; }
        public string empresa { get; set; }
    }
}
