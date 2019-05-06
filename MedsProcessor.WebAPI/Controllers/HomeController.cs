using System;
using System.Text;
using MedsProcessor.WebAPI.Controllers.v1;
using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, ApiVersionNeutral]
	public class HomeController : ControllerBase
	{
		static bool isFirstTimeRun = true;

		/// <summary>
		/// Displays initial information about the Web API.
		/// </summary>
		/// <returns>Returns a JSON formatted message with basic Web API info.</returns>
		[HttpGet("~/")]
		public ActionResult<ApiMessageResponse> Index()
		{
			// ! NOTE: this is not working, seems like IUrlHelper is not happy with api versioning:
			var statusUrl = Url.Action(
				nameof(ProcessorController.GetStatus),
				nameof(ProcessorController),
				new { version = 1 });

			var sb = new StringBuilder();

			sb.AppendLine(isFirstTimeRun ?
					"The HZZO meds-processor has started with the Web API launch!" :
					"Welcome to the HZZO meds-processor.")
				.AppendLine($" You can check the status at: { statusUrl }");

			isFirstTimeRun = false;

			return ApiResponse.ForMessage(sb.ToString());
		}
	}
}