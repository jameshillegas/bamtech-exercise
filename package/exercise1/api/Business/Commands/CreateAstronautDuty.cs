using Dapper;
using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Business.Exceptions;

namespace StargateAPI.Business.Commands;

public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
{
    public required string Name { get; set; }

    public required string Rank { get; set; }

    public required string DutyTitle { get; set; }

    public DateTime DutyStartDate { get; set; }
}

public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
{
    private readonly StargateContext _context;

    public CreateAstronautDutyHandler(StargateContext context)
    {
        _context = context;
    }
    public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim().ToUpper();

        var query = @"SELECT *
                FROM [Person] 
                WHERE @NormalizedName = NormalizedName";

        var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query, new { NormalizedName = normalizedName });

        if (person == null)
        {
            throw new ResourceNotFoundException("Person does not exist.");
        }

        query = @"SELECT * 
                FROM [AstronautDetail] ad 
                WHERE @PersonId = PersonId";

        var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query, new { PersonId = person.Id });

        if (astronautDetail == null)
        {
            astronautDetail = new AstronautDetail
            {
                PersonId = person.Id,
                CurrentDutyTitle = request.DutyTitle,
                CurrentRank = request.Rank,
                CareerStartDate = request.DutyStartDate.Date
            };

            if (request.DutyTitle == "RETIRED")
            {
                astronautDetail.CareerEndDate = request.DutyStartDate.Date;
            }

            await _context.AstronautDetails.AddAsync(astronautDetail);
        }
        else
        {
            astronautDetail.CurrentDutyTitle = request.DutyTitle;
            astronautDetail.CurrentRank = request.Rank;
            if (request.DutyTitle == "RETIRED")
            {
                astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
            }
            _context.AstronautDetails.Update(astronautDetail);
        }

        query = @"SELECT * 
            FROM [AstronautDuty] 
            WHERE @PersonId = PersonId 
            Order By DutyStartDate Desc";

        var astronautDuty = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDuty>(query, new { PersonId = person.Id });

        if (astronautDuty != null)
        {
            astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
            _context.AstronautDuties.Update(astronautDuty);
        }

        var newAstronautDuty = new AstronautDuty()
        {
            PersonId = person.Id,
            Rank = request.Rank,
            DutyTitle = request.DutyTitle,
            DutyStartDate = request.DutyStartDate.Date,
            DutyEndDate = null
        };

        await _context.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateAstronautDutyResult()
        {
            Id = newAstronautDuty.Id
        };
    }
}

public class CreateAstronautDutyResult
{
    public int Id { get; set; }
}

