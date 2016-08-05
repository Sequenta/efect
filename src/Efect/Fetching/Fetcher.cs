using System;
using System.Collections.Generic;
using System.Linq;

namespace Efect.Fetching
{
    public class Fetcher<T> : IFetcher<T> where T : class
    {
        private readonly IEnumerable<Func<IQueryable<T>, IQueryable<T>>> _properties;

        public Fetcher(params Func<IQueryable<T>, IQueryable<T>>[] properties)
        {
            _properties = properties;
        }

        public IQueryable<T> Fetch(IQueryable<T> source)
        {
            return _properties.Aggregate(source , (query, property) => property(query));
        }
    }
}