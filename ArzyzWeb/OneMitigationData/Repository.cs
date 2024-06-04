
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ArzyzWeb.OneMitigationData
{
    public class Repository
    {
        protected SqlConnection _context;
        protected SqlTransaction _transaction;
        protected SqlCommand CreateCommand(string query)
        {
            return new SqlCommand(query, _context, _transaction);
        }

        public string GetInClause(string values, string prefixParam, bool isUUID)
        {
            List<string> empresas = values.Split('|').ToList();
            string inClause = string.Empty;

            inClause = isUUID ? string.Join(",", empresas.Select((s, i) => $"UUID_TO_BIN(@{prefixParam}{i})"))
                : string.Join(",", empresas.Select((s, i) => $"@{prefixParam}{i}"));

            return inClause;
        }

        public void SetInValuesClause(string values, string prefixParam, SqlCommand cmd)
        {
            int indice = 0;
            foreach (var item in values.Split('|').ToList())
            {
                cmd.Parameters.AddWithValue($"@{prefixParam}{indice}", item);
                indice++;
            }
        }
        public string SetLikeValue(string text)
        {
            return $"%{text}%";
        }

    }
}
