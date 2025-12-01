using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;

namespace StargateAPI.Tests.Business.Queries;

public class GetAstronautDutiesByNameHandlerTests
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
        var now = DateTime.UtcNow.Date;
        conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (1, 'John Doe', 'JOHN DOE');");
        conn.Execute("INSERT INTO AstronautDetail (PersonId, CurrentRank, CurrentDutyTitle, CareerStartDate) VALUES (1, 'Commander', 'Mission Specialist', '" + now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
        conn.Execute("INSERT INTO AstronautDuty (PersonId, DutyStartDate, DutyTitle, Rank) VALUES (1, '" + now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Mission Specialist', 'Commander');");
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task Returns_Person_And_Duties_By_Name()
    {
        var handler = new GetAstronautDutiesByNameHandler(_context!);
        var result = await handler.Handle(new GetAstronautDutiesByName { Name = "John Doe" }, CancellationToken.None);

        Assert.That(result.Person, Is.Not.Null);
        Assert.That(result.Person!.Name, Is.EqualTo("John Doe"));
        Assert.That(result.AstronautDuties.Count, Is.EqualTo(1));
    }
}

