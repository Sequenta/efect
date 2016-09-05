using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Efect.Fetching;
using Microsoft.EntityFrameworkCore;

namespace Efect
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext DbContext;
        private readonly IFetcher<TEntity> _fetcher;

        public Repository(DbContext dbContext, IFetcher<TEntity> fetcher = null)
        {
            DbContext = dbContext;
            _fetcher = fetcher;
        }

        public void AddOne(TEntity entity)
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        public void AddMany(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().AddRange(entities);
        }

        public void UpdateOne(TEntity entity)
        {
            DbContext.Set<TEntity>().Update(entity);
        }

        public void UpdateMany(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().UpdateRange(entities);
        }

        public void DeleteOne(TEntity entity)
        {
            DbContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteMany(IEnumerable<TEntity> entities)
        {
            DbContext.Set<TEntity>().RemoveRange(entities);
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null)
        {
            var query = GetQuery(filter, fetcher);
            return query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null)
        {
            var query = GetQuery(filter, fetcher);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 10, IFetcher<TEntity> fetcher = null)
        {
            var query = GetQuery(filter, fetcher);
            return await query.Skip(page > 1 ? page * pageSize : 0).Take(pageSize).ToListAsync();
        }

        public async Task<TSelection> SelectOneAsync<TSelection>(Expression<Func<TEntity, TSelection>> selector, Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null)
        {
            var query = GetQuery(filter, fetcher);
            return await query.Select(selector).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TSelection>> SelectManyAsync<TSelection>(Expression<Func<TEntity, TSelection>> selector, Expression<Func<TEntity, bool>> filter = null, IFetcher<TEntity> fetcher = null)
        {
            var query = GetQuery(filter, fetcher);
            return await query.Select(selector).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter, new Fetcher<TEntity>());
            return await query.AnyAsync();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = GetQuery(filter, new Fetcher<TEntity>());
            return await query.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        private IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter, IFetcher<TEntity> fetcher)
        {
            fetcher = fetcher ?? _fetcher;
            var query = filter != null ? DbContext.Set<TEntity>().Where(filter) : DbContext.Set<TEntity>();
            if (fetcher != null)
                query = fetcher.Fetch(query);
            return query;
        }
    }
}