using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, ApiVersion("1"), Route("api/v{version:apiVersion}/[controller]")]
  [Produces("application/json")]
	abstract public class ApiV1ControllerBase : ControllerBase { }
}