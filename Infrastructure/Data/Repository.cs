using Microsoft.EntityFrameworkCore;
using ScaffoldHandler;
using Shared;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _dbContext;
        public Repository(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<(bool Success,string Message)> AddAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                _dbContext.Set<T>().Add(entity);
                await _dbContext.SaveChangesAsync();
                return (true, "Success");
            }
            catch (Exception ex)
            {
                if(ex.InnerException !=null) { return (false, $"Trouble happened! \n {ex.Message} {ex.InnerException}"); }
                return (false, $"Trouble happened! \n {ex.Message}");
            }
        }
        public async Task<(bool Success, string Message)> UpdateAsync<T>(T entity) where T : BaseEntity
        {
            try
            {
                var item = await _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == entity.Id);

                _dbContext.Entry(item).State = EntityState.Modified;
                _dbContext.Entry(item).CurrentValues.SetValues(entity);
                _dbContext.Entry(item).Property(x => x.CreatedBy).IsModified = false;
                _dbContext.Entry(item).Property(x => x.CreatedAt).IsModified = false;
                await _dbContext.SaveChangesAsync();

                return (true, "Success");
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }
        public async Task<(bool Success, string Message)> DeleteAsync<T>(Guid id) where T : BaseEntity
        {
            try
            {
                var item = await _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id == id);
                _dbContext.Remove<T>(item);
                await _dbContext.SaveChangesAsync();
                return (true, "Success");
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }

        public IQueryable<T> GetQueryable<T>(int takeMaxRows = 0, params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            var rItem = _dbContext.Set<T>().AsQueryable<T>();
            rItem = rItem.EagerLoadInclude(includes);
            if (takeMaxRows > 0)
            {
                rItem = rItem.Take(takeMaxRows).OrderBy(o => o.Id);
            }
            return rItem;
        }



    }
}
