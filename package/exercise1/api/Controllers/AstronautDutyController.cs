using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Exceptions;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AstronautDutyController : ControllerBase
{
    private readonly IMediator _mediator;
    public AstronautDutyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetAstronautDutiesByName(string name)
    {
        try
        {
            var result = await _mediator.Send(new GetAstronautDutiesByName() { Name = name });

            if (result == null)
            {
                return this.GetResponse(BaseResponse<GetAstronautDutiesByNameResult>.NotFound());
            }

            return this.GetResponse(BaseResponse<GetAstronautDutiesByNameResult>.Ok(result));
        }
        catch (Exception ex)
        {
            return this.GetResponse(BaseResponse<GetAstronautDutiesByNameResult>.ServerError(ex.Message));
        }
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
    {
        try
        {
            var result = await _mediator.Send(request);
            return this.GetResponse(BaseResponse<CreateAstronautDutyResult>.Ok(result));
        }
        catch (ValidationException vex)
        {
            return this.GetResponse(vex.ToBadRequestResponse<CreateAstronautDutyResult>());
        }
        catch (ResourceNotFoundException rnfx)
        {
            return this.GetResponse(BaseResponse<CreateAstronautDutyResult>.NotFound(rnfx.Message));
        }
        catch (Exception ex)
        {
            return this.GetResponse(BaseResponse<CreateAstronautDutyResult>.ServerError(ex.Message));
        }
    }
}
