using System.Linq;

namespace Efect.Fetching
{
    public interface IFetcher<T> where T : class 
    {
        IQueryable<T> Fetch(IQueryable<T> source);
    }
}