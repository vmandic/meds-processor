using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("~/")]
	public class AppController : ControllerBase
	{
		public ActionResult Index()
		{
			var startTime = DateTime.Now;
			// TODO: implement scraper and parser logic
			var totalTime = startTime - DateTime.Now;

			return Ok($"Done! Handler duration: {totalTime.Duration()}");
		}
	}
}