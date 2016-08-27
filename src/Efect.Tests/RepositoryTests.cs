using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Efect.Fetching;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Efect.Tests
{
    public class RepositoryTests : DatabaseFixture
    {
        [Theory, AutoData]
        public async Task AddOneTest(Person person)
        {
            var repository = new Repository<Person>(TestDatabase);

            repository.AddOne(person);
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                Expression<Func<Person, bool>> personMatcher = x => x.Id == person.Id && 
                                                                    x.Age == person.Age && 
                                                                    x.Email == person.Email && 
                                                                    x.Name == person.Name;
                var addedPerson = context.Persons.FirstOrDefault();

                addedPerson.Should().NotBeNull().And
                    .Match(personMatcher);
            }
        }

        [Theory, AutoData]
        public async Task AddManyTest(List<Person> persons)
        {
            var repository = new Repository<Person>(TestDatabase);

            repository.AddMany(persons);
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                var addedPersons = context.Persons.Count();

                addedPersons.Should().Be(persons.Count);
            }
        }


        [Theory, AutoData]
        public async Task UpdateOneTest(Person person, Address newAddress)
        {
            TestDatabase.Persons.Add(person);
            await TestDatabase.SaveChangesAsync();
            person.Address = newAddress;
            var repository = new Repository<Person>(TestDatabase);

            repository.UpdateOne(person);
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                var updatedPerson = context.Persons.Include(x => x.Address).ThenInclude(x => x.State).First();

                updatedPerson.Should().Match<Person>(x => x.Address.Id == newAddress.Id && 
                                                          x.Address.State.Id == newAddress.State.Id);
            }
        }

        [Theory, AutoData]
        public async Task UpdatManyTest(List<Person> persons, Address newAddress)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            persons.ForEach(x => x.Address = newAddress);
            var repository = new Repository<Person>(TestDatabase);

            repository.UpdateMany(persons);
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                var updatedPersons = context.Persons.Include(x => x.Address).ThenInclude(x => x.State).ToList();

                updatedPersons.Should().OnlyContain((x => x.Address.Id == newAddress.Id &&
                                                          x.Address.State.Id == newAddress.State.Id));
            }
        }

        [Theory, AutoData]
        public async Task DeleteOneTest(Person person)
        {
            TestDatabase.Persons.Add(person);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(TestDatabase);

            repository.DeleteOne(person);
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                var personsLeft = await context.Persons.CountAsync();

                personsLeft.Should().Be(0);
            }
        }

        [Theory, AutoData]
        public async Task DeleteManyTest(List<Person> persons)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(TestDatabase);

            repository.DeleteMany(new [] {persons[0], persons[2]});
            await repository.SaveChangesAsync();

            using (var context = new TestDatabaseContext())
            {
                var personsLeft = await context.Persons.CountAsync();

                personsLeft.Should().Be(1);
            }
        }

        [Theory, AutoData]
        public async Task FindOneAsyncWithoutParametersReturnsSingleElement(List<Person> persons)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());

            var foundPerson = await repository.FindOneAsync();

            foundPerson.Should().NotBeNull().And
                .Match<Person>(x => x.Address == null);
        }

        [Theory, AutoData]
        public async Task FindOneAsyncWithDefaultRepositoryFetcher(List<Person> persons)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var fetcher = Fetchers.Create<Person>(query => query.Include(x => x.Address));
            var repository = new Repository<Person>(new TestDatabaseContext(), fetcher);

            var foundPerson = await repository.FindOneAsync();

            foundPerson.Should().NotBeNull().And
                .Match<Person>(x => x.Address != null);
        }

        [Theory, AutoData]
        public async Task FindOneAsyncWithEmptyFetcherReturnsPlainEntity(List<Person> persons)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var fetcher = Fetchers.Create<Person>(query => query.Include(x => x.Address));
            var repository = new Repository<Person>(new TestDatabaseContext(), fetcher);

            var foundPerson = await repository.FindOneAsync(null, Fetchers.Create<Person>());

            foundPerson.Should().NotBeNull().And
                .Match<Person>(x => x.Address == null);
        }

        [Theory, AutoData]
        public async Task FindOneAsyncWithFetcherReturnsFullEntity(List<Person> persons)
        {
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());

            var fetcher = Fetchers.Create<Person>(query => query.Include(x => x.Address).ThenInclude(x => x.State));
            var foundPerson = await repository.FindOneAsync(null, fetcher);

            foundPerson.Should().NotBeNull().And
                .Match<Person>(x => x.Address != null && x.Address.State != null);
        }

        [Fact]
        public async Task FindManyAsyncReturnsMatchedEntities()
        {
            var fixture = new Fixture();
            var persons = fixture.CreateMany<Person>(5).ToList();
            persons[0].Age = 30;
            persons[2].Age = 30;
            persons[3].Age = 30;
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());
            Expression<Func<Person, bool>> filter = x => x.Age == 30;

            var foundPersons = await repository.FindManyAsync(filter, null);

            foundPersons.Should().HaveCount(3);
        }

        [Fact]
        public async Task FindManyAsyncWithPagingReturnsPagedEntities()
        {
            var fixture = new Fixture();
            var persons = fixture.CreateMany<Person>(30).ToList();
            foreach (var person in persons.Take(20))
            {
                person.Address.State.Abbreviation = "CA";
            }
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());
            Expression<Func<Person, bool>> filter = x => x.Address.State.Abbreviation == "CA";

            var firstPage = (await repository.FindManyAsync(filter, 1, 5)).ToList();
            var secondPage = (await repository.FindManyAsync(filter, 2, 5)).ToList();

            firstPage.Should().HaveSameCount(secondPage).And.NotContain(secondPage);
        }

        [Theory, AutoData]
        public async Task SelectOneAsyncTest(Person person)
        {
            TestDatabase.Persons.Add(person);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());

            var selection = await repository.SelectOneAsync(x => new {x.Name, x.Age, x.Email});

            selection.GetType().GetProperties().Select(x => x.Name).Should().Contain(new[] {"Name", "Age", "Email"});
        }

        [Fact]
        public async Task SelectManyAsyncTest()
        {
            var fixture = new Fixture();
            var persons = fixture.CreateMany<Person>(5).ToList();
            TestDatabase.Persons.AddRange(persons);
            await TestDatabase.SaveChangesAsync();
            var repository = new Repository<Person>(new TestDatabaseContext());

            var selection = await repository.SelectManyAsync(x => new { x.Name, x.Age, x.Email });

            var selectedProperties = new[] {"Name", "Age", "Email"};
            selection.Should().HaveCount(persons.Count).And
                .OnlyContain(x => x.GetType().GetProperties().Select(y => y.Name).All(y => selectedProperties.Contains(y)));
        }
    }
}