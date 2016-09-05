using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Efect.Fetching;

namespace Efect
{
    public interface IRepository<TEntity> where TEntity : class 
    {
        void AddOne(TEntity entity);
        void AddMany(IEnumerable<TEntity> entities);

        void UpdateOne(TEntity entity);
        void UpdateMany(IEnumerable<TEntity> entities);

        void DeleteOne(TEntity entity);
        void DeleteMany(IEnumerable<TEntity> entities);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null);
        Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null);
        Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter = null, int page = 0, int pageSize = 10, IFetcher<TEntity> fetcher = null);

        Task<TSelection> SelectOneAsync<TSelection>(Expression<Func<TEntity, TSelection>> selector, Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null);
        Task<IEnumerable<TSelection>> SelectManyAsync<TSelection>(Expression<Func<TEntity, TSelection>> selector, Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);

        Task SaveChangesAsync();
    }
}