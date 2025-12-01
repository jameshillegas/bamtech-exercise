using NUnit.Framework;
using StargateAPI.Business.Queries.Validators;
using StargateAPI.Business.Queries;

namespace StargateAPI.Tests.Business.Queries.Validators;

public class GetAstronautDutiesByNameValidatorTests
{
    [Test]
    public async Task GetAstronautDutiesByNameValidator_Should_Fail_When_Name_Is_Empty()
    {
        var validator = new GetAstronautDutiesByNameValidator();
        var result = await validator.ValidateAsync(new GetAstronautDutiesByName { Name = "" });
        Assert.That(result.IsValid, Is.False);
    }
}