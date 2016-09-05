using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Efect.Tests;

namespace Efect.Tests.Migrations
{
    [DbContext(typeof(TestDatabaseContext))]
    [Migration("20160803151910_TestEntities")]
    partial class TestEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("Efect.Tests.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("PostCode");

                    b.Property<int>("StateId");

                    b.Property<string>("Street");

                    b.HasKey("Id");

                    b.HasIndex("StateId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("Efect.Tests.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<int>("Age");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("Efect.Tests.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("Efect.Tests.Address", b =>
                {
                    b.HasOne("Efect.Tests.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Efect.Tests.Person", b =>
                {
                    b.HasOne("Efect.Tests.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
