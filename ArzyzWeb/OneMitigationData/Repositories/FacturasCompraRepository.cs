using ArzyzWeb.OneMitigationData.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ArzyzWeb.OneMitigationData.Repositories
{
    public class FacturasCompraRepository : Repository
    {
        private readonly string _table = "FacturasCompraH";      

        public FacturasCompraRepository(SqlConnection context, SqlTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }
        private FacturasCompraH CreateItem(DbDataReader reader)
        {
            var item = new FacturasCompraH()
            {
                ordenCompra = reader["ordenCompra"].ToString(),
                idFactura = reader["idFactura"].ToString(),
                fecha = DateTime.Parse(reader["fecha"].ToString()),
                cuentaProveedor = reader["cuentaProveedor"].ToString(),
                moneda = reader["moneda"].ToString(),
                impuestos = decimal.Parse(reader["impuestos"].ToString()),
                montototal = decimal.Parse(reader["montototal"].ToString()),
                condicionPago = reader["condicionPago"].ToString(),
                empresa = reader["empresa"].ToString(),
                grupoProveedor = reader["grupoProveedor"].ToString()                 
            };

            item.fecha_text = item.fecha.ToString("yyyy-MM-dd");

            return item;
        }

        public async Task<List<string>> GetEmpresas()
        {
            List<string> lista = new List<string>();
            SqlCommand cmd = CreateCommand($"select distinct empresa from {_table}");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lista.Add(reader["empresa"].ToString());
                }
            }

            return lista;
        }
        public async Task<List<int>> GetAnios()
        {
            List<int> lista = new List<int>();
            SqlCommand cmd = CreateCommand($"select distinct YEAR(fecha) as anio from {_table}");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lista.Add(int.Parse(reader["anio"].ToString()));
                }
            }

            return lista;

        }
        public async Task<List<FacturasCompraH>> GetFilter(string empresa,string OrdenCompra,string CuentaProveedor,
            string anio, int page, int pageSize)
        {
            List<FacturasCompraH> lista = new List<FacturasCompraH>();

            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fecha) in({GetInClause(anio, "pa", false)}) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in({GetInClause(empresa, "pe", false)})  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and ordenCompra = @OrdenCompra ";
            string whereCuenta = string.IsNullOrWhiteSpace(CuentaProveedor) ? "" : " and cuentaProveedor = @CuentaProveedor ";

            int StartRow = (page - 1) * pageSize + 1;
            int EndRow = page * pageSize;

            string query = @$"WITH Paginados AS(
                            SELECT [ordenCompra]
                                  ,[idFactura]
                                  ,[fecha]
                                  ,[cuentaProveedor]
                                  ,[moneda]
                                  ,[impuestos]
                                  ,[montototal]
                                  ,[condicionPago]
                                  ,[empresa]
                                  ,[grupoProveedor],
	                              ROW_NUMBER() OVER (ORDER BY fecha) AS RowNum
                              FROM {_table} where 1 = 1 {whereAnio} {whereEmpresa} {whereOrden} {whereCuenta}) 
                            select * FROM Paginados WHERE RowNum BETWEEN {StartRow} AND {EndRow} ORDER BY RowNum;";
                                   
            SqlCommand cmd = CreateCommand(query);

            if (!string.IsNullOrWhiteSpace(empresa))
                SetInValuesClause(empresa, "pe", cmd);

            if (!string.IsNullOrWhiteSpace(anio))
                SetInValuesClause(anio, "pa", cmd);

            if (!string.IsNullOrWhiteSpace(OrdenCompra))
                cmd.Parameters.AddWithValue("@OrdenCompra", OrdenCompra);

            if (!string.IsNullOrWhiteSpace(CuentaProveedor))
                cmd.Parameters.AddWithValue("@CuentaProveedor", CuentaProveedor);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lista.Add(CreateItem(reader));
                }
            }

            return lista;
        }
        public async Task<int> GetFilterCount(string empresa, string OrdenCompra, string CuentaProveedor, string anio)
        {
            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fecha) in({GetInClause(anio, "pa", false)}) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in({GetInClause(empresa, "pe", false)})  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and ordenCompra = @OrdenCompra ";
            string whereCuenta = string.IsNullOrWhiteSpace(CuentaProveedor) ? "" : " and cuentaProveedor = @CuentaProveedor ";
           
            var query = $"select count(*) total from {_table} where 1 = 1 {whereAnio} {whereEmpresa} {whereOrden} {whereCuenta} ";

            SqlCommand cmd = CreateCommand(query);

            if (!string.IsNullOrWhiteSpace(empresa))
                SetInValuesClause(empresa, "pe", cmd);

            if (!string.IsNullOrWhiteSpace(anio))
                SetInValuesClause(anio, "pa", cmd);

            if (!string.IsNullOrWhiteSpace(OrdenCompra))
                cmd.Parameters.AddWithValue("@OrdenCompra", OrdenCompra);

            if (!string.IsNullOrWhiteSpace(CuentaProveedor))
                cmd.Parameters.AddWithValue("@CuentaProveedor", CuentaProveedor);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return int.Parse(reader["total"].ToString());
                }
            }

            return 0;
        }
    }
}
