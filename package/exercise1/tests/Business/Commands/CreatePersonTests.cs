using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Tests.Business.Commands;

public class CreatePersonTests
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
    public async Task CreatePerson_Creates_A_Person()
    {
        var handler = new CreatePersonHandler(_context!);
        var result = await handler.Handle(new CreatePerson { Name = "Alice Johnson" }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));

        var personInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM Person WHERE Id = @Id", new { Id = result.Id });
        Assert.That(personInDb, Is.Not.Null);
        Assert.That(personInDb!.Name, Is.EqualTo("Alice Johnson"));
    }

    [Test]
    public void CreatePerson_Throws_When_Name_Exists()
    {
        var handler = new CreatePersonHandler(_context!);
        Assert.ThrowsAsync<BusinessRuleException>(async () =>
        {
            await handler.Handle(new CreatePerson { Name = "John Doe" }, CancellationToken.None);
        });
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}

