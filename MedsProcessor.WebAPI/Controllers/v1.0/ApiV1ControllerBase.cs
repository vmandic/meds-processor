using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers.v1_0
{
	[Authorize, ApiController, ApiVersion("1"), Route("api/v{apiVersion:apiVersion}")]
	[Consumes("application/json"), Produces("application/json")]
	[ProducesResponseType(200), ProducesResponseType(400)]
	[ProducesResponseType(401), ProducesResponseType(500)]
abstract public class ApiV1ControllerBase : ControllerBase { }
}