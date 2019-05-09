using System.Text;
using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiVersionNeutral]
	public class HomeController : ApiControllerBase
	{
		static bool isFirstTimeRun = true;

		/// <summary>
		/// Displays initial information about the Web API.
		/// </summary>
		/// <returns>Returns a JSON formatted message with basic Web API info.</returns>
		[HttpGet("~/")]
		public ActionResult<ApiMessageResponse> Index()
		{
			var v1StatusUrl = Url.Link("v1.0_Processor_GetStatus", new { apiVersion = "1.0" });

			var sb = new StringBuilder();

			sb.Append(isFirstTimeRun ?
					"The HZZO meds-processor has started with the Web API launch!" :
					"Welcome to the HZZO meds-processor.")
				.Append($" You can check the status at: { v1StatusUrl }");

			isFirstTimeRun = false;

			return ApiResponse.ForMessage(sb.ToString());
		}
	}
}