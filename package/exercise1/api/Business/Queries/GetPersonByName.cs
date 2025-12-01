using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPersonByName : IRequest<GetPersonByNameResult>
{
    public required string Name { get; set; } = string.Empty;
}

public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
{
    private readonly StargateContext _context;
    public GetPersonByNameHandler(StargateContext context)
    {
        _context = context;
    }

    public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
    {
        var query = @"
            SELECT 
                a.Id as PersonId, 
                a.Name, 
                b.CurrentRank, 
                b.CurrentDutyTitle, 
                b.CareerStartDate, 
                b.CareerEndDate 
            FROM [Person] a 
            LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id 
            WHERE @NormalizedName = a.NormalizedName";

        var person = await _context.Connection.QueryAsync<PersonAstronaut>(query, new { NormalizedName = request.Name.ToUpper() });

        return new GetPersonByNameResult
        {
            Person = person.FirstOrDefault()
        };
    }
}

public class GetPersonByNameResult
{
    public PersonAstronaut? Person { get; set; }
}


