using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(string where, string order);
        Task<object> GetAllPaginatedAsync(string? limit, string? offset, string? where, string? order);
        Task<object> Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task SaveAsync();
        void Dispose();
    }
}
