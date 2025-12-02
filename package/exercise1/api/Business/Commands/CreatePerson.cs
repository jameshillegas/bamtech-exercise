using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Business.Commands;

public class CreatePerson : IRequest<CreatePersonResult>
{
    public required string Name { get; set; } = string.Empty;
}

public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
{
    private readonly StargateContext _context;

    public CreatePersonHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
    {

        //check if person with same name exists
        var normalized = request.Name.ToUpperInvariant();
        var exists = await _context.People.AsNoTracking().AnyAsync(p => p.NormalizedName == normalized, cancellationToken);
        if (exists)
        {
            throw new BusinessRuleException("Person with the same name already exists.");
        }

        var newPerson = new Person()
        {
            Name = request.Name,
            NormalizedName = request.Name.ToUpperInvariant()
        };

        await _context.People.AddAsync(newPerson);

        await _context.SaveChangesAsync();

        return new CreatePersonResult()
        {
            Id = newPerson.Id
        };
    }
}

public class CreatePersonResult
{
    public int Id { get; set; }
}
