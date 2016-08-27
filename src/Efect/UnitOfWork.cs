using System.Threading.Tasks;
using Efect.Fetching;
using Microsoft.EntityFrameworkCore;

namespace Efect
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly DbContext Context;

        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public IRepository<TEntity> GetRepository<TEntity>(IFetcher<TEntity> fetcher) where TEntity : class
        {
            return new Repository<TEntity>(Context, fetcher);
        }

        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}