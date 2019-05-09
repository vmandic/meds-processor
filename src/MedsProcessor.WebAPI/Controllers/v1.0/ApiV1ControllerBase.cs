using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers.v1_0
{
	[Authorize, ApiVersion("1"), Route("api/v{apiVersion:apiVersion}")]
	[ProducesResponseType(401), ProducesResponseType(403)]
	public abstract class ApiV1ControllerBase : ApiControllerBase { }
}