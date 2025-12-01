using NUnit.Framework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Dapper;
using StargateAPI.Business.Data;
using StargateAPI.Business.Commands;
using FluentValidation;
using StargateAPI.Business.Exceptions;
using StargateAPI.Business.Commands.Validators;

namespace StargateAPI.Tests.Business.Commands;


public class CreatePersonValidatorTests
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
    }

    [Test]
    public async Task CreatePersonValidator_Should_Fail_When_Name_Is_Empty()
    {
        var validator = new CreatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new CreatePerson { Name = "" });

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task CreatePersonValidator_Should_Fail_When_Name_Exists()
    {
        var validator = new CreatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new CreatePerson { Name = "John Doe" });

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task CreatePersonValidator_Should_Fail_When_Name_Is_Too_Long()
    {
        var validator = new CreatePersonValidator(_context!);

        var longName = new string('A', 256);
        var result = await validator.ValidateAsync(new CreatePerson { Name = longName });

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == "Name"), Is.True);
    }

    [Test]
    public async Task CreatePersonValidator_Should_Pass_When_Valid()
    {
        var validator = new CreatePersonValidator(_context!);

        var result = await validator.ValidateAsync(new CreatePerson { Name = "John Smith" });

        Assert.That(result.IsValid, Is.True);
    }
}