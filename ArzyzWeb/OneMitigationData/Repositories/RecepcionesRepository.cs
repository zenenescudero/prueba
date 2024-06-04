using ArzyzWeb.OneMitigationData.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ArzyzWeb.OneMitigationData.Repositories
{
    public class RecepcionesRepository : Repository
    {
        private readonly string _table = "Recepciones";      

        public RecepcionesRepository(SqlConnection context, SqlTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }
        private Recepciones CreateItem(DbDataReader reader)
        {
            var item = new Recepciones()
            {
                IdRecepcion = reader["IdRecepcion"].ToString(),
                fechaRecepcion = DateTime.Parse(reader["fechaRecepcion"].ToString()),
                ordenCompra = reader["ordenCompra"].ToString(),
                articulo = reader["articulo"].ToString(),
                cantidad = decimal.Parse(reader["cantidad"].ToString()),
                unidad = reader["unidad"].ToString(),
                nombreArticulo = reader["nombreArticulo"].ToString(),
                configuracion = reader["configuracion"].ToString(),
                lote = reader["lote"].ToString(),
                empresa = reader["empresa"].ToString(),    
            };

            item.fecha_text = item.fechaRecepcion.ToString("yyyy-MM-dd");

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
            SqlCommand cmd = CreateCommand($"select distinct YEAR(fechaRecepcion) as anio from {_table}");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lista.Add(int.Parse(reader["anio"].ToString()));
                }
            }

            return lista;

        }

        public async Task<List<Recepciones>> GetFilter(string empresa,string OrdenCompra,
            string IdRecepcion,
            string anio, int page, int pageSize)
        {
            List<Recepciones> lista = new List<Recepciones>();

            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fechaRecepcion) in(@anio) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in(@empresa)  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and ordenCompra = @OrdenCompra ";
            string whereRecepcion = string.IsNullOrWhiteSpace(IdRecepcion) ? "" : " and IdRecepcion = @IdRecepcion ";

            int StartRow = (page - 1) * pageSize + 1;
            int EndRow = page * pageSize;

            string query = @$"WITH Paginados AS(
                            SELECT [IdRecepcion]
                                  ,[fechaRecepcion]
                                  ,[ordenCompra]
                                  ,[articulo]
                                  ,[cantidad]
                                  ,[unidad]
                                  ,[nombreArticulo]
                                  ,[configuracion]
                                  ,[lote]
                                  ,[empresa],
	                              ROW_NUMBER() OVER (ORDER BY fechaRecepcion) AS RowNum
                              FROM {_table} where 1 = 1 {whereAnio} {whereEmpresa} {whereOrden} {whereRecepcion}) 
                            select * FROM Paginados WHERE RowNum BETWEEN {StartRow} AND {EndRow} ORDER BY RowNum;";

            SqlCommand cmd = CreateCommand(query);

            if (!string.IsNullOrWhiteSpace(empresa))
               cmd.Parameters.AddWithValue("@empresa", empresa);

            if (!string.IsNullOrWhiteSpace(anio))
                cmd.Parameters.AddWithValue("@anio", anio);

            if (!string.IsNullOrWhiteSpace(OrdenCompra))
                cmd.Parameters.AddWithValue("@OrdenCompra", OrdenCompra);

            if (!string.IsNullOrWhiteSpace(IdRecepcion))
                cmd.Parameters.AddWithValue("@IdRecepcion", IdRecepcion);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    lista.Add(CreateItem(reader));
                }
            }

            return lista;
        }
        public async Task<int> GetFilterCount(string empresa,string OrdenCompra, string IdRecepcion, string anio)
        {
            string whereAnio = string.IsNullOrWhiteSpace(anio) ? "" : $" and YEAR(fechaRecepcion) in(@anio) ";
            string whereEmpresa = string.IsNullOrWhiteSpace(empresa) ? "" : $" and empresa in(@empresa)  ";

            string whereOrden = string.IsNullOrWhiteSpace(OrdenCompra) ? "" : " and ordenCompra = @OrdenCompra ";
            string whereRecepcion = string.IsNullOrWhiteSpace(IdRecepcion) ? "" : " and IdRecepcion = @IdRecepcion ";

            var query = $"select count(*) total from {_table} where 1 = 1 {whereAnio} {whereEmpresa} {whereOrden} {whereRecepcion} ";

            SqlCommand cmd = CreateCommand(query);

            if (!string.IsNullOrWhiteSpace(empresa))
                cmd.Parameters.AddWithValue("@empresa", empresa);

            if (!string.IsNullOrWhiteSpace(anio))
                cmd.Parameters.AddWithValue("@anio", anio);

            if (!string.IsNullOrWhiteSpace(OrdenCompra))
                cmd.Parameters.AddWithValue("@OrdenCompra", OrdenCompra);

            if (!string.IsNullOrWhiteSpace(IdRecepcion))
                cmd.Parameters.AddWithValue("@IdRecepcion", IdRecepcion);

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
