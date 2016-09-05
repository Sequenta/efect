using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Efect.Tests
{
    public class DatabaseFixture : IDisposable
    {
        protected TestDatabaseContext TestDatabase { get; }
        public DatabaseFixture()
        {
            TestDatabase = new TestDatabaseContext();
            TestDatabase.Database.Migrate();
        }

        public void Dispose()
        {
            var dataSourceFile = TestDatabase.Database.GetDbConnection().DataSource;
            TestDatabase.Dispose();
            File.Delete(dataSourceFile);
        }
    }
}