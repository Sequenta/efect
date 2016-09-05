using System;
using System.Linq;

namespace Efect.Fetching
{
    public static class Fetchers
    {
        public static IFetcher<T> Create<T>(params Func<IQueryable<T>, IQueryable<T>>[] properties) where T : class
        {
            return new Fetcher<T>(properties);
        }
    }
}