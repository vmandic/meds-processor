using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, ApiVersionNeutral, Route("[controller]")]
	public class ErrorController : ControllerBase
	{
		/// <summary>
		/// Catches and handles global exceptions by intercepting HTTP errors by provided HTTP status code in the query parameter.
		/// </summary>
		/// <param name="code">A HTTP status code indicating the specific error.</param>
		/// <returns>Returns a JSON formatted message which will contain exception details if possible.</returns>
		[HttpGet("{code}")]
		public IActionResult GetError(int code)
		{
			var exFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			return exFeat == null
				? ApiHttpResponse.ForCode(code)
				: ApiHttpResponse.ForData(exFeat.Error, code, exFeat.Error.Message);
		}
	}
}