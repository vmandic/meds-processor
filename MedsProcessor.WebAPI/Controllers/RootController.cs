using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("~/")]
	public class RootController : ControllerBase
	{
		[HttpGet]
		public ActionResult Index()
		{
			return Ok("The Web API meds-processor is running... You can force start the processor at: ~/processor/run/true");
		}
	}
}