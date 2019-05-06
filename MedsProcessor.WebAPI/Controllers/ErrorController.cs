using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, ApiVersionNeutral, Route("api/[controller]")]
	public class ErrorController : ControllerBase
	{
		/// <summary>
		/// Catches and handles global exceptions by intercepting HTTP errors by provided HTTP status code in the query parameter.
		/// </summary>
		/// <param name="code">A HTTP status code indicating the specific error.</param>
		/// <returns>Returns a JSON formatted message which will contain exception details if possible.</returns>
		[HttpGet("{code}")]
		[Produces(typeof(ApiDataResponse<System.Exception>))]
		public ActionResult<ApiHttpResponse> GetError(int code)
		{
			var exFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			return exFeat == null
				? ApiResponse.ForCode(code)
				: ApiResponse.ForData(exFeat.Error, code, exFeat.Error.Message);
		}
	}
}