using ArzyzWeb.OneMitigationData.Entities;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ArzyzWeb.OneMitigationData.Repositories
{
    public class UserRepository : Repository
    {
        private readonly string _table = "Usuarios";      

        public UserRepository(SqlConnection context, SqlTransaction transaction)
        {
            _context = context;
            _transaction = transaction;
        }
        private User CreateItem(DbDataReader reader)
        {
            return new User()
            {
                id = int.Parse(reader["id"].ToString()),
                nombre = reader["nombre"].ToString(),
                email = reader["email"].ToString(),
                puesto = reader["puesto"].ToString(),
                password = reader["password"].ToString(),
            };
        }
        public async Task<User> GetUserLogin(string email)
        {
            string query = @$"select * from {_table} where email = @email";

            SqlCommand cmd = CreateCommand(query);

            cmd.Parameters.AddWithValue("@email", email);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return CreateItem(reader);
                }
            }

            return null;
        }     

    }
}
