using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("~/")]
	public class RootController : ControllerBase
	{
		static bool isFirstTimeRun = true;

		[HttpGet]
		public ActionResult Index()
		{
			var getStatusUrl = Url.RouteUrl("Processor_GetStatus");

			var sb = new StringBuilder();

			sb.AppendLine(isFirstTimeRun ?
				"The HZZO meds-processor has started with the Web API launch!" :
				"Welcome to the HZZO meds-processor. You can check the status at: ")
				.AppendLine(getStatusUrl);

			isFirstTimeRun = false;

			return Ok(ApiResponse.ForMessageOk(sb.ToString()));
		}
	}
}