using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Commands.Validators;

public class CreateAstronautDutyValidator : AbstractValidator<CreateAstronautDuty>
{
    private readonly StargateContext _context;

    public CreateAstronautDutyValidator(StargateContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters.") //common sense limit, not defined in requirements
            .MustAsync(async (name, ct) =>
            {
                return await _context.People.AsNoTracking().AnyAsync(z => z.Name == name, ct);
            }).WithMessage("Person does not exist.");

        RuleFor(x => x.Rank)
            .NotEmpty().WithMessage("Rank is required.")
            .MaximumLength(100).WithMessage("Rank must not exceed 100 characters.");

        RuleFor(x => x.DutyTitle)
            .NotEmpty().WithMessage("Duty Title is required.")
            .MaximumLength(100).WithMessage("Duty Title must not exceed 100 characters.");
        RuleFor(x => x.DutyStartDate)
            .NotEmpty().WithMessage("Duty Start Date is required.");

        //unsure of this rule.
        // A note exists '1. A Person who has not had an astronaut assignment will not have Astronaut records."
        // and there was a pre-processor step that checked `var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate);`
        // but these two things are not consistent - need clarification on the requirement.

        // RuleFor(x => x)
        //     .MustAsync(async (request, ct) =>
        //     {
        //         var existingDuty = await _context.AstronautDuties
        //             .AsNoTracking()
        //             .FirstOrDefaultAsync(z => z.Person.Name == request.Name && z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate, ct);

        //         return existingDuty is null;
        //     })
        //     .WithMessage("An astronaut duty with the same title and start date already exists for that person.");
    }
}