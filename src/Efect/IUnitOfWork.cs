using System;
using System.Threading.Tasks;
using Efect.Fetching;

namespace Efect
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>(IFetcher<TEntity> fetcher) where TEntity : class;
        Task SaveChangesAsync();
    }
}