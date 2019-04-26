using System.Net;
using MedsProcessor.WebAPI.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("[controller]")]
	public class ErrorController : ControllerBase
	{
		[HttpGet("{code}")]
		public IActionResult GetError(int code)
		{
			var exFeat = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			return new ObjectResult(exFeat != null ?
				ApiResponse.ForData(code, exFeat.Error, exFeat.Error.Message) :
				ApiResponse.ForCode(code));
		}
	}
}