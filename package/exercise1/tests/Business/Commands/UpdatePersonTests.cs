using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Tests.Business.Commands;

public class UpdatePersonTests
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
    }

    [Test]
    public async Task UpdatePerson_Updates_A_Person()
    {
        var handler = new UpdatePersonHandler(_context!);
        var result = await handler.Handle(new UpdatePerson { Id = 1, Name = "Alice Johnson" }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Name, Is.EqualTo("Alice Johnson"));

        //verify in db
        var personInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM Person WHERE Id = @Id", new { Id = result.Id });
        Assert.That(personInDb, Is.Not.Null);
        Assert.That(personInDb!.Name, Is.EqualTo("Alice Johnson"));
    }

    [Test]
    public void UpdatePerson_Throws_When_Person_Not_Found()
    {
        var handler = new UpdatePersonHandler(_context!);
        Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
        {
            await handler.Handle(new UpdatePerson { Id = 999, Name = "Non Existent" }, CancellationToken.None);
        });
    }

    [Test]
    public void UpdatePerson_Throws_When_Name_Exists()
    {
        var handler = new UpdatePersonHandler(_context!);
        var conn = _context!.Connection;
        //put a 2nd name in the db to cause conflict
        conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (2, 'Robert Losier', 'ROBERT LOSIER');");

        Assert.ThrowsAsync<BusinessRuleException>(async () =>
        {
            // Attempt to update person with Id 1 to a name that already exists
            await handler.Handle(new UpdatePerson { Id = 1, Name = "Robert Losier" }, CancellationToken.None);
        });
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}

