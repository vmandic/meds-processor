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
		private readonly Processor _processor;

		public ProcessorController(Processor processor)
		{
			this._processor = processor;
		}

		[HttpGet("run")]
		public async Task<ActionResult> Run()
		{
			return Ok(await _processor.Run());
		}
	}
}