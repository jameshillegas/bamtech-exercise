using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Exceptions;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {

            var result = new GetAstronautDutiesByNameResult();

            var query = @"SELECT 
                    a.Id as PersonId, 
                    a.Name, 
                    b.CurrentRank, 
                    b.CurrentDutyTitle, 
                    b.CareerStartDate, 
                    b.CareerEndDate 
                FROM [Person] a 
                LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id 
                WHERE @NormalizedName = a.NormalizedName";

            var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query, new { NormalizedName = request.Name.ToUpper() });

            if (person == null)
            {
                throw new ResourceNotFoundException("Person does not exist.");
            }

            result.Person = person;

            query = @"SELECT * 
            FROM [AstronautDuty] 
            WHERE @PersonId = PersonId 
            Order By DutyStartDate Desc";

            var duties = await _context.Connection.QueryAsync<AstronautDuty>(query, new { person.PersonId });

            result.AstronautDuties = duties.ToList();

            return result;

        }
    }
    public class GetAstronautDutiesByNameResult
    {
        public PersonAstronaut? Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
