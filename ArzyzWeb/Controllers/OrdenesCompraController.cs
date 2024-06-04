using ArzyzWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.IO;
using System;
using System.Threading.Tasks;

namespace ArzyzWeb.Controllers
{
    [Authorize]
    public class OrdenesCompraController : BaseController
    {
        private readonly ILogger<OrdenesCompraController> _logger;

        public OrdenesCompraController(ILogger<OrdenesCompraController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetEmpresas()
        {
            var lista = await _context.OrdenesCompra().GetEmpresas();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAnios()
        {
            var lista = await _context.OrdenesCompra().GetAnios();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAll([FromQuery] string empresa, [FromQuery]string OrdenCompra,
            [FromQuery] string CuentaProveedor,
            [FromQuery] string nombreProveedor,
            [FromQuery] string Anio,
           [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var lista = await _context.OrdenesCompra().GetFilter(empresa,OrdenCompra, CuentaProveedor, nombreProveedor, Anio, page, pageSize);

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCount([FromQuery] string empresa, [FromQuery] string OrdenCompra,
            [FromQuery] string CuentaProveedor,
            [FromQuery] string nombreProveedor,
            [FromQuery] string Anio)
        {

            var contador = await _context.OrdenesCompra().GetFilterCount(empresa, OrdenCompra, CuentaProveedor, nombreProveedor, Anio);

            return Json(contador);
        }


        [HttpPost]
        public async Task<FileResult> ExportExcel([FromQuery] string empresa, [FromQuery] string OrdenCompra,
          [FromQuery] string CuentaProveedor,
          [FromQuery] string nombreProveedor,
          [FromQuery] string Anio)
        {
            try
            {
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                var reportList = await _context.OrdenesCompra().GetFilter(empresa, OrdenCompra, CuentaProveedor, nombreProveedor, Anio, 1, Helpers.pageSizeMax);

                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Data");
                    int celda = 1;

                    worksheet.Cells[1, celda++].Value = "Orden Compra";
                    worksheet.Cells[1, celda++].Value = "Cuenta Proveedor";
                    worksheet.Cells[1, celda++].Value = "Nombre Proveedor";
                    worksheet.Cells[1, celda++].Value = "Fecha";
                    worksheet.Cells[1, celda++].Value = "Moneda";

                    worksheet.Cells[1, celda++].Value = "Artículo";
                    worksheet.Cells[1, celda++].Value = "Nombre Artículo";
                    worksheet.Cells[1, celda++].Value = "Condición Pago";
                    worksheet.Cells[1, celda++].Value = "Precio Unitario";
                    worksheet.Cells[1, celda++].Value = "Cantidad";

                    worksheet.Cells[1, celda++].Value = "Unidad";
                    worksheet.Cells[1, celda++].Value = "Num. Linea";
                    worksheet.Cells[1, celda++].Value = "Impuestos";
                    worksheet.Cells[1, celda++].Value = "Empresa";
                    worksheet.Cells[1, celda++].Value = "Monto";

                    worksheet.Cells[1, celda++].Value = "Tipo Proveedor";

                    int row = 2;

                    foreach (var item in reportList)
                    {
                        celda = 1;

                        worksheet.Cells[row, celda++].Value = item.OrdenCompra;
                        worksheet.Cells[row, celda++].Value = item.CuentaProveedor;
                        worksheet.Cells[row, celda++].Value = item.nombreProveedor;
                        worksheet.Cells[row, celda++].Value = item.fecha_text;
                        worksheet.Cells[row, celda++].Value = item.moneda;

                        worksheet.Cells[row, celda++].Value = item.articulo;
                        worksheet.Cells[row, celda++].Value = item.nombreArticulo;
                        worksheet.Cells[row, celda++].Value = item.condicionPago;
                        worksheet.Cells[row, celda++].Value = item.precioUnitario;
                        worksheet.Cells[row, celda++].Value = item.cantidad;

                        worksheet.Cells[row, celda++].Value = item.unidad;
                        worksheet.Cells[row, celda++].Value = item.numLinea;
                        worksheet.Cells[row, celda++].Value = item.impuestos;
                        worksheet.Cells[row, celda++].Value = item.Empresa;
                        worksheet.Cells[row, celda++].Value = item.monto;

                        worksheet.Cells[row, celda++].Value = item.tipoProveedor;

                        row++;
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "OrdenesCompra.xlsx");
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(Path.Combine(pathTemp, "log_error.txt")))
                {
                    sw.WriteLine("Error Reporte");
                    sw.WriteLine(ex.Message.ToString());
                }

                return null;
            }

        }

    }
}
