using System;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using MedsProcessor.WebAPI.Models;
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
		public async Task<ActionResult<ApiMessageResponse>> Run(bool force = false)
		{
			return Ok(ApiResponse.ForMessageOk(await _processor.Run(force)));
		}

		[HttpGet("status", Name = "Processor_GetStatus")]
		public ActionResult<ApiDataResponse<HzzoDataProcessorStatus>> GetStatus()
		{
			return Ok(ApiResponse.ForDataOk(_processor.GetStatus()));
		}

		[HttpGet("clear-data", Name = "Processor_ClearData")]
		public ActionResult<ApiMessageResponse> ClearData()
		{
			var dataCleared = _processor.ClearData();

			return Ok(ApiResponse.ForMessageOk(dataCleared
				? "Processor data cleared! You now must rerun the processor to continue using the API."
				: "Processor is running... Data can not be cleared during processing! Try again later."));
		}
	}
}