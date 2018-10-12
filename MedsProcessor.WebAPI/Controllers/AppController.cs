using System;
using System.Threading.Tasks;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("api/[controller]")]
	public class AppController : ControllerBase
	{
		public async Task<ActionResult> Index()
		{
			var startTime = DateTime.Now;
			// TODO: implement scraper and parser logic
			var totalTime = startTime - DateTime.Now;

			return Ok($"Done! Handler duration: {totalTime.Duration()}");
		}
	}
}