using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces
{
    public interface IRepository
    {
        Task<(bool Success, string Message)> AddAsync<T>(T entity) where T : BaseEntity;
        Task<(bool Success, string Message)> UpdateAsync<T>(T entity) where T : BaseEntity;
        Task<(bool Success, string Message)> DeleteAsync<T>(Guid id) where T : BaseEntity;
        IQueryable<T> GetQueryable<T>(int takeMaxRows = 0, params Expression<Func<T, object>>[] includes) where T : BaseEntity;
        Task<T> GetByIdAsync<T>(Guid id) where T : BaseEntity;
    }
}
