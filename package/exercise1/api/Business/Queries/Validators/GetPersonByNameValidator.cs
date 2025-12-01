using FluentValidation;

namespace StargateAPI.Business.Queries.Validators;

public class GetPersonByNameValidator : AbstractValidator<GetPersonByName>
{
    public GetPersonByNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must be provided.");
    }
}