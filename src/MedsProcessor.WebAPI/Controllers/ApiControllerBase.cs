using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
		[ApiController, Route("api")]
		[Consumes("application/json"), Produces("application/json")]
		[ProducesResponseType(406), ProducesResponseType(426)]
		[ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
    public abstract class ApiControllerBase : ControllerBase { }
}