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


public class CreateAstronautDutyValidatorTests
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
    }

    [Test]
    public async Task CreateAstronautDutyValidator_Should_Fail_When_Person_Does_Not_Exist()
    {
        var validator = new CreateAstronautDutyValidator(_context!);
        var result = await validator.ValidateAsync(new CreateAstronautDuty { Name = "Non Existent", DutyTitle = "Pilot", Rank = "Lieutenant", DutyStartDate = DateTime.UtcNow });
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateAstronautDutyValidator_Should_Faild_When_Name_Is_Empty()
    {
        var validator = new CreateAstronautDutyValidator(_context!);
        var result = await validator.ValidateAsync(new CreateAstronautDuty { Name = "", DutyTitle = "Pilot", Rank = "Lieutenant", DutyStartDate = DateTime.UtcNow });
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateAstronautDutyValidator_Should_Fail_When_DutyTitle_Is_Empty()
    {
        var validator = new CreateAstronautDutyValidator(_context!);
        var result = await validator.ValidateAsync(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "", Rank = "Lieutenant", DutyStartDate = DateTime.UtcNow });
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateAstronautDutyValidator_Should_Fail_When_Rank_Is_Empty()
    {
        var validator = new CreateAstronautDutyValidator(_context!);
        var result = await validator.ValidateAsync(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "", DutyStartDate = DateTime.UtcNow });
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateAstronautDutyValidator_Should_Pass_When_Valid()
    {
        var validator = new CreateAstronautDutyValidator(_context!);
        var result = await validator.ValidateAsync(new CreateAstronautDuty { Name = "John Doe", DutyTitle = "Pilot", Rank = "Lieutenant", DutyStartDate = DateTime.UtcNow.AddDays(1) });
        Assert.That(result.IsValid, Is.True);
    }
}