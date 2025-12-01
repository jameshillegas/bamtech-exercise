using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Commands.Validators;

public class UpdatePersonValidator : AbstractValidator<UpdatePerson>
{
    private readonly StargateContext _context;

    public UpdatePersonValidator(StargateContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters."); //common sense limit, not defined in requirements

        RuleFor(x => x)
            .MustAsync(IsNameInUseByAnotherPerson).WithMessage("Person with the same name already exists.");
    }

    private async Task<bool> IsNameInUseByAnotherPerson(UpdatePerson person, CancellationToken ct)
    {
        var normalized = person.Name.ToUpperInvariant();
        return !await _context.People.AsNoTracking().AnyAsync(p => p.Id != person.Id && p.NormalizedName == normalized, ct);
    }
}
