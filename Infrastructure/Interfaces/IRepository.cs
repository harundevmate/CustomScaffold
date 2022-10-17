using Shared;
using System.Linq.Expressions;
namespace Infrastructure.Interfaces
{
    public interface IRepository
    {
        Task<(bool Success, string Message)> AddAsync<T>(T entity,bool AutoGenerateId = true) where T : BaseEntity;
        Task<(bool Success, string Message)> UpdateAsync<T>(T entity) where T : BaseEntity;
        Task<(bool Success, string Message)> DeleteAsync<T>(string id) where T : BaseEntity;
        IQueryable<T> GetQueryable<T>(int takeMaxRows = 0, params Expression<Func<T, object>>[] includes) where T : BaseEntity;
        Task<T> GetByIdAsync<T>(string id) where T : BaseEntity;
    }
}
