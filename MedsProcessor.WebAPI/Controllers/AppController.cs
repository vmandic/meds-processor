using System;
using System.Linq;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Scraper;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("~/")]
	public class AppController : ControllerBase
	{
		public async Task<ActionResult> Index(
			[FromServices] HzzoHtmlScraper scraper)
		{
			var startTime = DateTime.Now;
			// TODO: implement scraper and parser logic
			var meds = await scraper.Run();
			// meds = await parser.Run(meds);

			var totalTime = startTime - DateTime.Now;

			return Ok(
				$"Done! Handler duration: {totalTime.Duration()}" +
				Environment.NewLine +
				Environment.NewLine +
				string.Join(Environment.NewLine, meds.Select(x => x.FileName))
			);
		}

		[HttpGet("/root-path")]
		public string RootPath([FromServices] AppPathsInfo apInfo) =>
			"path: " + apInfo.ApplicationRootPath;
	}
}