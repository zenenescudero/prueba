using ArzyzWeb.OneMitigationData.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ArzyzWeb.OneMitigationData.Repositories
{
    public class OrdenesCompraRepository : Repository
    {
        private readonly string _table = "OrdenesCompra";      

        public OrdenesCompraRepository(SqlConnection context, SqlTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }
        private OrdenesCompra CreateItem(DbDataReader reader)
        {
            var item = new OrdenesCompra()
            {
                OrdenCompra = reader["OrdenCompra"].ToString(),
                CuentaProveedor = reader["CuentaProveedor"].ToString(),
                nombreProveedor = reader["nombreProveedor"].ToString(),
                fecha = DateTime.Parse(reader["fecha"].ToString()),
                moneda = reader["moneda"].ToString(),
                articulo = reader["articulo"].ToString(),
                nombreArticulo = reader["nombreArticulo"].ToString(),
                condicionPago = reader["condicionPago"].ToString(),
                precioUnitario = decimal.Parse(reader["precioUnitario"].ToString()),
                cantidad = decimal.Parse(reader["cantidad"].ToString()),
                unidad = reader["unidad"].ToString(),
                numLinea = long.Parse(reader["numLinea"].ToString()),
                impuestos = decimal.Parse(reader["impuestos"].ToString()),
                Empresa = reader["Empresa"].ToString(),
                monto = decimal.Parse(reader["monto"].ToString()),
                tipoProveedor = reader["tipoProveedor"] != DBNull.Value ? (int?)int.Parse(reader["tipoProveedor"].ToString()) : null
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
        public async Task<List<OrdenesCompra>> GetFilter(string empresa,string OrdenCompra,string CuentaProveedor,string nombreProveedor,
            string anio, int page, int pageSize)
        {
            List<OrdenesCompra> lista = new List<OrdenesCompra>();

            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fecha) in({GetInClause(anio, "pa", false)}) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in({GetInClause(empresa, "pe", false)})  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and OrdenCompra = @OrdenCompra ";
            string whereCuenta = string.IsNullOrWhiteSpace(CuentaProveedor) ? "" : " and CuentaProveedor = @CuentaProveedor ";
            string whereNombre = string.IsNullOrWhiteSpace(nombreProveedor) ? "" : $" and nombreProveedor like {SetLikeValue(nombreProveedor)} ";

            int StartRow = (page - 1) * pageSize + 1;
            int EndRow = page * pageSize;

            string query = @$"WITH Paginados AS(
                            SELECT  [OrdenCompra]
                                  ,[CuentaProveedor]
                                  ,[nombreProveedor]
                                  ,[fecha]
                                  ,[moneda]
                                  ,[articulo]
                                  ,[nombreArticulo]
                                  ,[condicionPago]
                                  ,[precioUnitario]
                                  ,[cantidad]
                                  ,[unidad]
                                  ,[numLinea]
                                  ,[impuestos]
                                  ,[Empresa]
                                  ,[monto]
                                  ,[tipoProveedor],
	                              ROW_NUMBER() OVER (ORDER BY fecha) AS RowNum
                              FROM {_table} where 1 = 1 {whereAnio} {whereOrden} {whereEmpresa} {whereCuenta} {whereNombre}) 
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
        public async Task<int> GetFilterCount(string empresa,string OrdenCompra, 
            string CuentaProveedor, string nombreProveedor,
            string anio)
        {
            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fecha) in({GetInClause(anio, "pa", false)}) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in({GetInClause(empresa, "pe", false)})  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and OrdenCompra = @OrdenCompra ";
            string whereCuenta = string.IsNullOrWhiteSpace(CuentaProveedor) ? "" : " and CuentaProveedor = @CuentaProveedor ";
            string whereNombre = string.IsNullOrWhiteSpace(nombreProveedor) ? "" : $" and nombreProveedor like {SetLikeValue(nombreProveedor)} ";

            var query = $"select count(*) total from {_table} where 1 = 1 {whereAnio} {whereEmpresa} {whereOrden} {whereCuenta} {whereNombre}";

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
