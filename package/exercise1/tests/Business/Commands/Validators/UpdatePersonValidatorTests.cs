using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Commands.Validators;

namespace StargateAPI.Tests.Business.Commands;

public class UpdatePersonValidatorTests
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
        conn.Execute("INSERT INTO Person (Id, Name, NormalizedName) VALUES (2, 'Jane Doe', 'JANE DOE');");
    }

    [Test]
    public async Task UpdatePersonValidator_Should_Fail_When_Name_Is_Empty()
    {
        var validator = new UpdatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new UpdatePerson { Id = 1, Name = "" });

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task UpdatePersonValidator_Should_Fail_When_Updated_Name_Already_Exists()
    {
        var validator = new UpdatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new UpdatePerson { Id = 1, Name = "Jane Doe" });
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdatePersonValidator_Should_Fail_When_Name_Is_Too_Long()
    {
        var validator = new UpdatePersonValidator(_context!);

        var longName = new string('A', 256);
        var result = await validator.ValidateAsync(new UpdatePerson { Id = 1, Name = longName });

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task UpdatePersonValidator_Should_Pass_When_Valid()
    {
        var validator = new UpdatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new UpdatePerson { Id = 1, Name = "Johnson Smithson" });
        Assert.That(result.IsValid, Is.True);
    }
}