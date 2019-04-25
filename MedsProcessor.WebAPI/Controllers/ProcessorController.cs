using System;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
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

		[HttpGet("run/{force?}", Name = "Processor_Run")]
		public async Task<ActionResult> Run(bool force = false)
		{
			return Ok(await _processor.Run(force));
		}

		[HttpGet("status", Name = "Processor_GetStatus")]
		public ActionResult<HzzoDataProcessorStatusVm> GetStatus()
		{
			return Ok(_processor.GetStatus());
		}

		[HttpGet("clear-data", Name = "Processor_ClearData")]
		public ActionResult ClearData()
		{
			var dataCleared = _processor.ClearData();

			return Ok(dataCleared
				? "Processor data cleared! You now must rerun the processor to continue using the API."
				: "Processor is running... Data can not be cleared during processing! Try again later.");
		}
	}
}