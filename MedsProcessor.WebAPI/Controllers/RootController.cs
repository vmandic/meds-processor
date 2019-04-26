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
			var clearDataUrl = Url.RouteUrl("Processor_ClearData");
			var getStatusUrl = Url.RouteUrl("Processor_GetStatus");
			var rerunUrl = Url.RouteUrl("Processor_Run", new { force = true });

			var sb = new StringBuilder();

			sb.AppendLine(isFirstTimeRun ?
				"The HZZO meds-processor has started with the Web API launch!" :
				"Welcome to the HZZO meds-processor.").AppendLine();

			isFirstTimeRun = false;

			sb.Append("You can check the status at: ").AppendLine(getStatusUrl);
			sb.Append("You can clear the loaded data at: ").AppendLine(clearDataUrl);
			sb.Append("You can restart the processor at: ").AppendLine(rerunUrl);

			return Ok(ApiResponse.ForMessageOk(sb.ToString()));
		}
	}
}