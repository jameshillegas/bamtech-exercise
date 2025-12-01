using FluentValidation;

namespace StargateAPI.Business.Queries.Validators;

public class GetAstronautDutiesByNameValidator : AbstractValidator<GetAstronautDutiesByName>
{
    public GetAstronautDutiesByNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must be provided.");
    }
}