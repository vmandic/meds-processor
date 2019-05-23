using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiVersionNeutral, Route("api/[controller]")]
	public class AuthController : ApiControllerBase
	{
		private readonly IJwtAuthService jwtService;
		public AuthController(IJwtAuthService jwtService)
		{
			this.jwtService = jwtService;
		}

		[AllowAnonymous, HttpPost("token")]
		public ActionResult<AuthTokenResponse> RequestToken([FromBody] AuthTokenRequest request)
		{
			var(authenticatedSuccessfully, token) = jwtService.IssueToken(request);

			return authenticatedSuccessfully
				? ApiResponse.ForData(
						new AuthTokenResponse(token),
						message: "Access token issued successfully.")
				: ApiResponse.ForData(
						new AuthTokenResponse(),
						StatusCodes.Status401Unauthorized,
						message: "An invalid or unauthorized Client ID was provided.");
		}
	}
}