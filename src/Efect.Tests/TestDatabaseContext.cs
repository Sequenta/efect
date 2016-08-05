using Microsoft.EntityFrameworkCore;

namespace Efect.Tests
{
    public class TestDatabaseContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasOne(pt => pt.Address)
                .WithMany()
                .HasForeignKey(pt => pt.AddressId);
            modelBuilder.Entity<Address>()
                .HasOne(pt => pt.State)
                .WithMany()
                .HasForeignKey(pt => pt.StateId);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename = Test.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}