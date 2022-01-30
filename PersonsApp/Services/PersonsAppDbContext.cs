using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonsApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PersonsApp.Services
{
    public class PersonsAppDbContext : IdentityDbContext
    {
        public PersonsAppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .IsRequired()
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Person>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Person>()
                .Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<Person>()
                .Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<Person>()
                .Property(p => p.PhoneNumber)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Person>()
                .Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Person>()
                .Property(p => p.Birthday)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(p => p.ModifyDate)
                .IsRequired();

            modelBuilder.Entity<RefreshToken>()
                .HasKey(x => x.Token);

            modelBuilder.Entity<RefreshToken>()
                .Property(x => x.Token)
                .IsRequired();

            //var json = File.ReadAllText("persons.json");
            //var option = new JsonSerializerOptions();
            //option.PropertyNameCaseInsensitive = true;
            //var persons = JsonSerializer.Deserialize<List<Person>>(json, option);
            //modelBuilder.Entity<Person>().HasData(persons);
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
