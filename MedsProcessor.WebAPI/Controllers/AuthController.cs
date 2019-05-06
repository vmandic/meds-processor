using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, ApiVersionNeutral, Route("api/[controller]")]
	[ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
	public class AuthController : ControllerBase
	{
		private readonly IJwtAuthService jwtService;
		public AuthController(IJwtAuthService jwtService)
		{
			this.jwtService = jwtService;
		}

		[AllowAnonymous, HttpPost("token")]
		[Consumes("application/json"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public ActionResult RequestToken([FromBody] AuthTokenRequest request)
		{
			var(authenticatedSuccessfully, token) = jwtService.IssueToken(request);

			return authenticatedSuccessfully
				? (ActionResult) Ok(token)
				: Unauthorized();
		}
	}
}