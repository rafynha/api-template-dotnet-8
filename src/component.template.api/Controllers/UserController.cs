using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Filters;
using component.template.api.domain.Helpers;
using component.template.api.domain.Interfaces.Common;
using component.template.api.domain.Models.Common;
using component.template.api.domain.Models.External;
using component.template.api.domain.Models.External.User;
using component.template.api.domain.Models.External.User.Commands;
using component.template.api.domain.Models.External.User.Queries;
using Microsoft.AspNetCore.Mvc;

namespace component.template.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<GetAllUsersResponse>>))]
        public async Task<ActionResult> GetAll([FromQuery] GetAllUsersQuery request)
        {
            _logger.LogInformation($"Iniciando endpoint GetAll do controller {typeof(UserController)} --> Params: Username={request.Username}, Email={request.Email}, IsActive={request.IsActive}, PageNumber={request.PageNumber}, PageSize={request.PageSize}");

            if (ModelState.IsValid)
                return Ok(await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<GetUserByIdResponse>))]
        public async Task<ActionResult> GetById(long id)
        {
            _logger.LogInformation($"Iniciando endpoint GetById do controller {typeof(UserController)} --> Params: Id={id}");

            var request = new GetUserByIdQuery { Id = id };

            if (ModelState.IsValid)
                return Ok(await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IApiResponse<CreateUserResponse>))]
        public async Task<ActionResult> Post([FromBody] CreateUserCommand request)
        {
            _logger.LogInformation($"Iniciando endpoint Post do controller {typeof(UserController)} --> Params: {string.Empty/*Newtonsoft.Json.JsonConvert.SerializeObject(request)*/}");

            if (ModelState.IsValid)
                return Created(string.Empty, await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<UpdateUserResponse>))]
        public async Task<ActionResult> Put([FromBody] UpdateUserCommand request)
        {
            _logger.LogInformation($"Iniciando endpoint Put do controller {typeof(UserController)} --> Params: {string.Empty/*Newtonsoft.Json.JsonConvert.SerializeObject(request)*/}");

            if (ModelState.IsValid)
                return Ok(await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<DeleteUserResponse>))]
        public async Task<ActionResult> Delete(long id)
        {
            _logger.LogInformation($"Iniciando endpoint Delete do controller {typeof(UserController)} --> Params: Id={id}");

            var request = new DeleteUserCommand { Id = id };

            if (ModelState.IsValid)
                return Ok(await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }
    }
}