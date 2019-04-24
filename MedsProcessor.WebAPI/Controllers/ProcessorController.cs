using System;
using System.Threading.Tasks;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("[controller]")]
	public class ProcessorController : ControllerBase
	{
		private readonly HzzoDataProcessor _processor;

		public ProcessorController(HzzoDataProcessor processor)
		{
			this._processor = processor;
		}

		[HttpGet("run/{force?}")]
		public async Task<ActionResult> Run(bool force = false)
		{
			return Ok(await _processor.Run(force));
		}
	}
}