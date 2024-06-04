
using ArzyzWeb.OneMitigationData.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace ArzyzWeb.OneMitigationData
{
    public class OneMitigationContext : IDisposable
    {
        public string ConnectionString { get; set; }
        private SqlConnection _context { get; set; }
        private SqlTransaction _transaction { get; set; }

        protected UserRepository userRepository { get; set; }
        protected OrdenesCompraRepository _ordenesCompraRepository { get; set; }
        protected FacturasCompraRepository _facturasRepository {  get; set; }
        protected RecepcionesRepository _recepcionesRepository { get; set; }
              
        public OneMitigationContext()
        {
            var AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            string BConfig = AppSetting["ConnectionStrings:ArzyzDb"].ToString();
            ConnectionString = BConfig;
            _context = new SqlConnection(ConnectionString);
            _context.Open();
            _transaction = _context.BeginTransaction();
        }

        public UserRepository Usuarios()
        {
            return userRepository == null ? new UserRepository(_context, _transaction) : userRepository;
        }
        public OrdenesCompraRepository OrdenesCompra()
        {
            return _ordenesCompraRepository == null ? new OrdenesCompraRepository(_context, _transaction) : _ordenesCompraRepository;
        }
        public FacturasCompraRepository Facturas()
        {
            return _facturasRepository == null ? new FacturasCompraRepository(_context, _transaction) : _facturasRepository;
        }
        public RecepcionesRepository Recepciones()
        {
            return _recepcionesRepository == null ? new RecepcionesRepository(_context, _transaction) : _recepcionesRepository;
        }

        public async Task SaveChanges()
        {
            await _transaction.CommitAsync();
        }

        public void SaveChangesDirect()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            if (_context != null)
            {
                _context.Close();
                _context.Dispose();
            }
        }
    }
}
