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
		private readonly JwtAuthService authService;
		public AuthController(JwtAuthService authService)
		{
			this.authService = authService;
		}

		[AllowAnonymous, HttpPost("token")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public ActionResult<string> RequestToken([FromBody] AuthTokenRequest request)
		{
			var(authenticatedSuccessfully, token) = authService.IssueToken(request);

			if (authenticatedSuccessfully)
			{
				return Ok(token);
			}

			return Unauthorized();
		}
	}
}