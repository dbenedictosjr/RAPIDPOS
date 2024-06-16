using POS.Domain;
using POS.Infrastructure.Interfaces;
using POS.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure
{
    internal class AppDbContext : IDisposable
    {
        private readonly string _connectionString;

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IRepository<PS_DOC_HDR> _pS_DOC_HDRRepository;
        public IRepository<PS_DOC_HDR> PS_DOC_HDRRepositories =>
            _pS_DOC_HDRRepository ??= new Repository<PS_DOC_HDR>(_connectionString);

        public void Dispose()
        {
            // Dispose repositories if needed
            (_pS_DOC_HDRRepository as IDisposable)?.Dispose();
        }
    }
}
