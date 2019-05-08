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
		/// Catches and handles global exceptions by intercepting HTTP errors specified through the URL.
		/// </summary>
		/// <returns>Returns a JSON formatted message which will contain exception details if the framework can resolve the latest error.</returns>
		[HttpGet, Produces(typeof(ApiDataResponse<System.Exception>))]
		public ActionResult<ApiHttpResponse> GetError()
		{
			var exFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			return exFeat == null
				? ApiResponse.ForMessage("Internal server error details unavailable.", 500)
				: ApiResponse.ForData(exFeat.Error, 500, exFeat.Error.Message);
		}
	}
}