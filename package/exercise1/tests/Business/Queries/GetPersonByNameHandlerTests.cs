using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetPersonByNameHandlerTests
    {
        private StargateContext? _context;

        [SetUp]
        public void Setup()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseSqlite(connection)
                .Options;

            // Ensure schema exists
            _context = new StargateContext(options);
            _context.Database.EnsureCreated();

            //seed data
            var conn = _context.Connection;
            conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (1, 'John Doe', 'JOHN DOE');");
            conn.Execute("INSERT INTO AstronautDetail (PersonId, CurrentRank, CurrentDutyTitle, CareerStartDate) VALUES (1, 'Commander', 'Mission Specialist', CURRENT_TIMESTAMP);");
            conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (2, 'Sam Smith', 'SAM SMITH');");
            conn.Execute("INSERT INTO AstronautDetail (PersonId, CurrentRank, CurrentDutyTitle, CareerStartDate) VALUES (2, 'Lieutenant', 'Pilot', CURRENT_TIMESTAMP);");
        }

        [Test]
        public async Task Returns_PersonAstronaut_By_Name_CaseInsensitive()
        {
            var handler = new GetPersonByNameHandler(_context!);
            var result = await handler.Handle(new GetPersonByName { Name = "sam smith" }, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Person, Is.Not.Null);
            Assert.That(result.Person!.PersonId, Is.EqualTo(2));
            Assert.That(result.Person!.Name, Is.EqualTo("Sam Smith"));
            Assert.That(result.Person!.CurrentDutyTitle, Is.EqualTo("Pilot"));
            Assert.That(result.Person!.CurrentRank, Is.EqualTo("Lieutenant"));
        }

        [Test]
        public async Task Returns_Null_When_Person_Not_Found()  
        {
            var handler = new GetPersonByNameHandler(_context!);
            var result = await handler.Handle(new GetPersonByName { Name = "Non Existent" }, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Person, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}
