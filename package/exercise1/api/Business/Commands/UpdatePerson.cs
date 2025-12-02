using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Business.Commands;

public class UpdatePerson : IRequest<UpdatePersonResult>
{
    public required int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
}

public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
{
    private readonly StargateContext _context;

    public UpdatePersonHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        var person = await _context.People.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (person == null)
        {
            throw new ResourceNotFoundException($"Person not found.");
        }

        var nameInUse = await _context.People.AnyAsync(p => p.Id != request.Id && p.NormalizedName == request.Name.ToUpperInvariant(), cancellationToken);
        if (nameInUse)
        {
            throw new BusinessRuleException("Person with the same name already exists.");
        }

        person.Name = request.Name;
        person.NormalizedName = request.Name.ToUpperInvariant();
        _context.People.Update(person);
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdatePersonResult
        {
            Id = person.Id,
            Name = person.Name,
        };
    }
}

public class UpdatePersonResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
}

