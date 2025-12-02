using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using FluentValidation;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Tests.Business.Commands;

public class CreateAstronautDutyTests
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
        var now = DateTime.UtcNow;
        conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (1, 'John Doe', 'JOHN DOE');");
        conn.Execute("INSERT INTO AstronautDetail (PersonId, CurrentRank, CurrentDutyTitle, CareerStartDate) VALUES (1, 'Commander', 'Mission Specialist', '" + now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
        conn.Execute("INSERT INTO AstronautDuty (PersonId, DutyStartDate, DutyTitle, Rank) VALUES (1, '" + now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'Mission Specialist', 'Commander');");
    }

    [Test]
    public async Task CreateAstronautDuty_Creates_A_Duty()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        var result = await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Mission Specialist", Rank = "Commander", DutyStartDate = DateTime.UtcNow }, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(2));

        var dutyInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDuty WHERE Id = @Id", new { Id = result.Id });
        Assert.That(dutyInDb, Is.Not.Null);
        Assert.That(dutyInDb!.DutyTitle, Is.EqualTo("Mission Specialist"));
        Assert.That(dutyInDb!.Rank, Is.EqualTo("Commander"));
    }

    [Test]
    public async Task CreateAstronautDuty_Updates_AstronautDetail_CurrentDuty_And_Rank()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        var result = await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = DateTime.UtcNow }, CancellationToken.None);

        var personInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDetail WHERE PersonId = @Id", new { Id = 1 });
        Assert.That(personInDb, Is.Not.Null);
        Assert.That(personInDb!.CurrentDutyTitle, Is.EqualTo("Pilot"));
        Assert.That(personInDb!.CurrentRank, Is.EqualTo("Captain"));
    }

    [Test]
    public async Task CreateAstronautDuty_Sets_DutyStartDate_Correctly()
    {
        var dutyStartDate = DateTime.UtcNow.AddDays(2).Date;
        var handler = new CreateAstronautDutyHandler(_context!);
        var result = await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = dutyStartDate }, CancellationToken.None);

        var dutyInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDuty WHERE Id = @Id", new { Id = result.Id });
        Assert.That(dutyInDb, Is.Not.Null);
        //convert the duty start date to DateTime to avoid precision issues
        var dutyStartDateInDb = DateTime.Parse(dutyInDb!.DutyStartDate.ToString());
        Assert.That(dutyStartDateInDb, Is.EqualTo(dutyStartDate).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task CreateAstronautDuty_Sets_DutyEndDate_Null_For_New_Duty()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        var result = await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = DateTime.UtcNow }, CancellationToken.None);

        var dutyInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDuty WHERE Id = @Id", new { Id = result.Id });
        Assert.That(dutyInDb, Is.Not.Null);
        Assert.That(dutyInDb!.DutyEndDate, Is.Null);
    }

    [Test]
    public async Task CreateAstronautDuty_Sets_Previous_Duty_End_Date()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        var now = DateTime.UtcNow.AddDays(1).Date;
        await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = now }, CancellationToken.None);

        var previousDutyInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDuty WHERE Id = @Id", new { Id = 1 });
        Assert.That(previousDutyInDb, Is.Not.Null);
        Assert.That(previousDutyInDb!.DutyEndDate, Is.Not.Null);

        //A Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date when a new Astronaut Duty is received for a Person.
        var dutyEndDateInDb = DateTime.Parse(previousDutyInDb.DutyEndDate.ToString());
        var expectedDutyEndDate = now.Date.AddDays(-1);
        Assert.That(dutyEndDateInDb, Is.EqualTo(expectedDutyEndDate).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void CreateAstronautDuty_Throws_When_Person_Not_Found()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
        {
            var result = await handler.Handle(new CreateAstronautDuty { Name = "Non Existent", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = DateTime.UtcNow }, CancellationToken.None);
        });
    }

    [Test]
    public async Task CreateAstronautDuty_Sets_CareerEndDate_When_Retired()
    {
        var handler = new CreateAstronautDutyHandler(_context!);
        var now = DateTime.UtcNow.Date.AddDays(1);
        await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "RETIRED", Rank = "Captain", DutyStartDate = now }, CancellationToken.None);

        var personInDb = await _context!.Connection.QueryFirstOrDefaultAsync("SELECT * FROM AstronautDetail WHERE PersonId = @Id", new { Id = 1 });
        Assert.That(personInDb, Is.Not.Null);
        Assert.That(personInDb!.CurrentDutyTitle, Is.EqualTo("RETIRED"));
        Assert.That(personInDb!.CareerEndDate, Is.Not.Null);

        //A Person's Career End Date is one day before the Retired Duty Start Date.
        var careerEndDateInDb = DateTime.Parse(personInDb.CareerEndDate.ToString());
        var expectedCareerEndDate = now.Date.AddDays(-1);
        Assert.That(careerEndDateInDb, Is.EqualTo(expectedCareerEndDate).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void CreateAstronautDuty_Throws_When_New_DutyStartDate_Before_Latest_Existing_DutyStartDate()
    {
        //TODO: This test might be invalid based on review of the requirement.

        var handler = new CreateAstronautDutyHandler(_context!);
        var pastDate = DateTime.UtcNow.AddDays(-20); //earlier than existing duty start date

        Assert.ThrowsAsync<BusinessRuleException>(async () =>
        {
            var result = await handler.Handle(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Captain", DutyStartDate = pastDate }, CancellationToken.None);
        });
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
}

