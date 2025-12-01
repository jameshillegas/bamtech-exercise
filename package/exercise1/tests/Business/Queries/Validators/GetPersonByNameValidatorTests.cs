using NUnit.Framework;
using StargateAPI.Business.Queries.Validators;
using StargateAPI.Business.Queries;

namespace StargateAPI.Tests.Business.Queries.Validators;

public class GetPersonByNameValidatorTests
{
    [Test]
    public async Task GetPersonByNameValidator_Should_Fail_When_Name_Is_Empty()
    {
        var validator = new GetPersonByNameValidator();
        var result = await validator.ValidateAsync(new GetPersonByName { Name = "" });
        Assert.That(result.IsValid, Is.False);
    }
}