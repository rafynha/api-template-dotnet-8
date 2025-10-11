using Aurora.Mediator;
using component.template.api.domain.Exceptions;
using component.template.api.domain.Filters;
using component.template.api.domain.Helpers;
using component.template.api.domain.Interfaces.Common;
using component.template.api.domain.Models.Common;
using component.template.api.domain.Models.External;
using component.template.api.domain.Models.External.User;
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
        //[ResponseFilterFactory]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<IEnumerable<GetUserByIdResponse>>))]
        public async Task<ActionResult> Get([FromQuery] GetUserByIdQuery request)
        {
            _logger.LogInformation($"Iniciando endpoint Put do controller {typeof(UserController)} --> Params: {string.Empty/*Newtonsoft.Json.JsonConvert.SerializeObject(request)*/}");

            if (ModelState.IsValid)
                return Ok(await _mediator.Send(request));
            else
                throw new InvalidModelStateException($"ModelState do controller {typeof(UserController)} inválido! --> Params:");
        }
    }
}