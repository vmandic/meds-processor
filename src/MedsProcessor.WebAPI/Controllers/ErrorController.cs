using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiVersionNeutral, Route("api/[controller]")]
	public class ErrorController : ApiControllerBase
	{
		/// <summary>
		/// Catches and handles global exceptions by intercepting HTTP errors specified through the URL.
		/// </summary>
		/// <returns>Returns a JSON formatted message which will contain exception details if the framework can resolve the latest error.</returns>
		[HttpGet, Produces(typeof(ApiDataResponse<System.Exception>))]
		public ActionResult<ApiMessageResponse> GetError()
		{
			var exFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			return ApiResponse.ForMessage(
				exFeat == null
					? "Internal server error details unavailable."
					: exFeat.Error.Message,
				500);
		}
	}
}