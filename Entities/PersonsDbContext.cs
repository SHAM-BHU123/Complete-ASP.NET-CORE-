
using Microsoft.EntityFrameworkCore;
using System;
namespace Entities
{
    public class PersonsDbContext:DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }
        public PersonsDbContext(DbContextOptions dbContextOptions):base 
        (dbContextOptions)
        {
         
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Person>().ToTable("Persons");

            /*Seed data to Countries */

            string countryjson=File.ReadAllText("countries.json");
            List<Country> countries= System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countryjson);

            foreach(Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }



            /*Seed data to Persons */

            string personjson = File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personjson);

            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

        }

        public List<Person> StoredProcedureGetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]")
            .ToList();
        }
    }
}
