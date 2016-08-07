using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    }
}