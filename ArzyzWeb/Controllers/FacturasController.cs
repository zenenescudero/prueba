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
    public class FacturasController : BaseController
    {
        private readonly ILogger<FacturasController> _logger;

        public FacturasController(ILogger<FacturasController> logger)
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
            var lista = await _context.Facturas().GetEmpresas();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAnios()
        {
            var lista = await _context.Facturas().GetAnios();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAll([FromQuery] string empresa, [FromQuery]string OrdenCompra,
            [FromQuery] string CuentaProveedor,
            [FromQuery] string Anio,
           [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var lista = await _context.Facturas().GetFilter(empresa,OrdenCompra, CuentaProveedor, Anio, page, pageSize);

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCount([FromQuery] string empresa, [FromQuery] string OrdenCompra,
            [FromQuery] string CuentaProveedor,
            [FromQuery] string Anio)
        {

            var contador = await _context.Facturas().GetFilterCount(empresa,OrdenCompra, CuentaProveedor, Anio);

            return Json(contador);
        }


        [HttpPost]
        public async Task<FileResult> ExportExcel([FromQuery] string empresa, [FromQuery] string OrdenCompra,
          [FromQuery] string CuentaProveedor,
          [FromQuery] string Anio)
        {
            try
            {
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                var reportList = await _context.Facturas().GetFilter(empresa,OrdenCompra, CuentaProveedor, Anio, 1, Helpers.pageSizeMax);

                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Data");
                    int celda = 1;

                    worksheet.Cells[1, celda++].Value = "Orden Compra";
                    worksheet.Cells[1, celda++].Value = "Id Factura";
                    worksheet.Cells[1, celda++].Value = "Fecha";
                    worksheet.Cells[1, celda++].Value = "Cuenta Proveedor";
                    worksheet.Cells[1, celda++].Value = "Moneda";

                    worksheet.Cells[1, celda++].Value = "Impuestos";
                    worksheet.Cells[1, celda++].Value = "Monto";
                    worksheet.Cells[1, celda++].Value = "Condición Pago";
                    worksheet.Cells[1, celda++].Value = "Empresa";
                    worksheet.Cells[1, celda++].Value = "Grupo Proveedor";
                                      

                    int row = 2;

                    foreach (var item in reportList)
                    {
                        celda = 1;

                        worksheet.Cells[row, celda++].Value = item.ordenCompra;
                        worksheet.Cells[row, celda++].Value = item.idFactura;
                        worksheet.Cells[row, celda++].Value = item.fecha_text;
                        worksheet.Cells[row, celda++].Value = item.cuentaProveedor;
                        worksheet.Cells[row, celda++].Value = item.moneda;

                        worksheet.Cells[row, celda++].Value = item.impuestos;
                        worksheet.Cells[row, celda++].Value = item.montototal;
                        worksheet.Cells[row, celda++].Value = item.condicionPago;
                        worksheet.Cells[row, celda++].Value = item.empresa;
                        worksheet.Cells[row, celda++].Value = item.grupoProveedor;


                        row++;
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "FacturasCompra.xlsx");
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
