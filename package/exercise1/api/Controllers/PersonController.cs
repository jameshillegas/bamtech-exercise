using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Exceptions;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople() { });
                return this.GetResponse(BaseResponse<GetPeopleResult>.Ok(result));
            }
            catch (Exception ex)
            {
                return this.GetResponse(BaseResponse<GetPeopleResult>.ServerError(ex.Message));
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName() { Name = name });

                if (result == null || result.Person == null)
                {
                    return this.GetResponse(BaseResponse<GetPersonByNameResult>.NotFound());
                }

                return this.GetResponse(BaseResponse<GetPersonByNameResult>.Ok(result));
            }
            catch (ValidationException vex)
            {
                return this.GetResponse(vex.ToBadRequestResponse<GetPersonByNameResult>());
            }
            catch (Exception ex)
            {
                return this.GetResponse(BaseResponse<GetPersonByNameResult>.ServerError(ex.Message));
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson() { Name = name });
                return this.GetResponse(BaseResponse<CreatePersonResult>.Ok(result));
            }
            catch (ValidationException vex)
            {
                return this.GetResponse(vex.ToBadRequestResponse<CreatePersonResult>());
            }
            catch (Exception ex)
            {
                return this.GetResponse(BaseResponse<CreatePersonResult>.ServerError(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new UpdatePerson() { Id = id, Name = name });
                return this.GetResponse(BaseResponse<UpdatePersonResult>.Ok(result));
            }
            catch (ValidationException vex)
            {
                return this.GetResponse(vex.ToBadRequestResponse<UpdatePersonResult>());
            }
            catch (ResourceNotFoundException rnfx)
            {
                return this.GetResponse(BaseResponse<UpdatePersonResult>.NotFound(rnfx.Message));
            }
            catch (Exception ex)
            {
                return this.GetResponse(BaseResponse<UpdatePersonResult>.ServerError(ex.Message));
            }
        }
    }
}