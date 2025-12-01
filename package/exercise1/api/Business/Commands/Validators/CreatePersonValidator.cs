using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Commands.Validators;

public class CreatePersonValidator : AbstractValidator<CreatePerson>
{
    private readonly StargateContext _context;

    public CreatePersonValidator(StargateContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters.") //common sense limit, not defined in requirements
            .MustAsync(IsNameUnique).WithMessage("Person with the same name already exists.");
    }

    private async Task<bool> IsNameUnique(string name, CancellationToken ct)
    {
        var normalized = name.ToUpperInvariant();
        return await _context.People.AsNoTracking().AllAsync(p => p.NormalizedName != normalized, ct);
    }
}
