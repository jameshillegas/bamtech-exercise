using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries;

public class GetPeople : IRequest<GetPeopleResult> { }

public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
{
    public readonly StargateContext _context;
    public GetPeopleHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
    {
        var query = @"
                SELECT 
                    p.Id as PersonId, 
                    p.Name, 
                    ad.CurrentRank, 
                    ad.CurrentDutyTitle, 
                    ad.CareerStartDate, 
                    ad.CareerEndDate 
                FROM [Person] p 
                LEFT JOIN [AstronautDetail] ad on ad.PersonId = p.Id";

        var people = await _context.Connection.QueryAsync<PersonAstronaut>(query);

        return new GetPeopleResult
        {
            People = people.ToList()
        };
    }
}
public class GetPeopleResult
{
    public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut>();
}
