# efect
Efect is repository pattern implementation on top of Entity Framework.

## Usage

```
var repository = new Repository<Person>(DbContext);
repository.AddOne(person);
await repository.SaveChangesAsync();
```
The following CRUD operations are implemented in Repository class:
* `AddOne`
* `AddMany`
* `UpdateOne`
* `UpdateMany`
* `DeleteOne`
* `DeleteMany`
* `FindOne`
* `FindMany`

Other operations:
* `SelectOne`
* `SelectMany`
* `Count`
* `Any`

## Fetching

Use fetchers to get complex fields from database
You can create them by using constructor

`var fetcher = new Fetcher<Person>(query => query.Include(x => x.Address));`

or `Fetchers` factory class

`var fetcher = Fetchers.Create<Person>(query => query.Include(x => x.Address));`

and pass it to one of `FindOne` or `FindMany` methods

```
var repository = new Repository<Person>(new TestDatabaseContext());
Expression<Func<Person, bool>> filter = x => x.Age == 30;
var foundPersons = await repository.FindManyAsync(filter, fetcher);
```
To avoid passing the same fetcher for every query, you can set default fetcher for the repository:
```
var fetcher = new Fetcher<Person>(query => query.Include(x => x.Address));
var repository = new Repository<Person>(DbContext, fetcher);
```
Now repository will apply this fetcher for all `FindOne` or `FindMany` queries.

NOTE: if you pass custom fetcher to repository with default fetcher, it will override default fetcher for this particular query
