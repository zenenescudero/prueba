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
    public class RecepcionesController : BaseController
    {
        private readonly ILogger<RecepcionesController> _logger;

        public RecepcionesController(ILogger<RecepcionesController> logger)
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
            var lista = await _context.Recepciones().GetEmpresas();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAnios()
        {
            var lista = await _context.Recepciones().GetAnios();

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAll([FromQuery] string empresa, [FromQuery]string OrdenCompra,
            [FromQuery] string IdRecepcion,
            [FromQuery] string Anio,
           [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            var lista = await _context.Recepciones().GetFilter(empresa, OrdenCompra, IdRecepcion, Anio, page, pageSize);

            return Json(lista);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCount([FromQuery] string empresa, [FromQuery] string OrdenCompra,
            [FromQuery] string IdRecepcion,
            [FromQuery] string Anio)
        {

            var contador = await _context.Recepciones().GetFilterCount(empresa, OrdenCompra, IdRecepcion, Anio);

            return Json(contador);
        }


        [HttpPost]
        public async Task<FileResult> ExportExcel([FromQuery] string empresa, [FromQuery] string OrdenCompra,
          [FromQuery] string IdRecepcion,
          [FromQuery] string Anio)
        {
            try
            {
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


                var reportList = await _context.Recepciones().GetFilter(empresa, OrdenCompra, IdRecepcion, Anio, 1, Helpers.pageSizeMax);

                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Data");
                    int celda = 1;

                    worksheet.Cells[1, celda++].Value = "Id Recepción";
                    worksheet.Cells[1, celda++].Value = "Fecha de Recepción";
                    worksheet.Cells[1, celda++].Value = "Orden Compra";
                    worksheet.Cells[1, celda++].Value = "Artículo";
                    worksheet.Cells[1, celda++].Value = "Cantidad";

                    worksheet.Cells[1, celda++].Value = "Unidad";
                    worksheet.Cells[1, celda++].Value = "Nombre Artículo";
                    worksheet.Cells[1, celda++].Value = "Configuración";
                    worksheet.Cells[1, celda++].Value = "Lote";
                    worksheet.Cells[1, celda++].Value = "Empresa";
                                      

                    int row = 2;

                    foreach (var item in reportList)
                    {
                        celda = 1;

                        worksheet.Cells[row, celda++].Value = item.IdRecepcion;
                        worksheet.Cells[row, celda++].Value = item.fecha_text;
                        worksheet.Cells[row, celda++].Value = item.ordenCompra;
                        worksheet.Cells[row, celda++].Value = item.articulo;
                        worksheet.Cells[row, celda++].Value = item.cantidad;

                        worksheet.Cells[row, celda++].Value = item.unidad;
                        worksheet.Cells[row, celda++].Value = item.nombreArticulo;
                        worksheet.Cells[row, celda++].Value = item.configuracion;
                        worksheet.Cells[row, celda++].Value = item.lote;
                        worksheet.Cells[row, celda++].Value = item.empresa;

                        row++;
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "Recepciones.xlsx");
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
