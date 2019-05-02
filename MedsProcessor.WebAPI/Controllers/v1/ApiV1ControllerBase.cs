using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers.v1
{
	[ApiController, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
  [Produces("application/json")]
	[ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
	abstract public class ApiV1ControllerBase : ControllerBase { }
}