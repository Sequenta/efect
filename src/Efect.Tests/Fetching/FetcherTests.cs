using System.Collections.Generic;
using System.Linq;
using Efect.Fetching;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Efect.Tests.Fetching
{
    public class FetcherTests : DatabaseFixture
    {
        [Theory, AutoData]
        public void EmptyFetcherReturnsEntityWithoutProperties(Person person)
        {
            var fetcher = new Fetcher<Person>();
            TestDatabase.Set<Person>().Add(person);
            TestDatabase.SaveChanges();

            using (var context = new TestDatabaseContext())
            {
                var query = context.Set<Person>().Where(x => x.Id == person.Id);
                var fetchedPerson = fetcher.Fetch(query).First();

                fetchedPerson.Should().NotBeNull().And
                    .Match<Person>(x => x.Address == null);
            } 
        }

        [Theory, AutoData]
        public void FetcherReturnsEntityWithProperties(Person person)
        {
            var fetcher = new Fetcher<Person>(x => x.Include(y => y.Address).ThenInclude(y => y.State));
            TestDatabase.Set<Person>().Add(person);
            TestDatabase.SaveChanges();

            using (var context = new TestDatabaseContext())
            {
                var query = context.Set<Person>().Where(x => x.Id == person.Id);
                var fetchedPerson = fetcher.Fetch(query).First();

                fetchedPerson.Should().NotBeNull().And
                    .Match<Person>(x => x.Address != null && x.Address.State != null);
            }
        }

        [Theory, AutoData]
        public void OrderFetcherReturnsOrderedEntities(List<Person> persons)
        {
            var fetcher = new Fetcher<Person>(x => x.OrderBy(y => y.Id));
            TestDatabase.Persons.AddRange(persons);
            TestDatabase.SaveChanges();

            using (var context = new TestDatabaseContext())
            {
                var personIds = persons.Select(x => x.Id);
                var query = context.Set<Person>().Where(x => personIds.Contains(x.Id));
                var fetchedPersons = fetcher.Fetch(query);

                fetchedPersons.Should().BeInAscendingOrder(x => x.Id);
            }
        }
    }
}