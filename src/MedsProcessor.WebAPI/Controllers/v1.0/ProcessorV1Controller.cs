using System.Threading.Tasks;
using MedsProcessor.WebAPI.Core;
using MedsProcessor.WebAPI.Models;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers.v1_0
{
	public class ProcessorV1Controller : ApiV1ControllerBase
	{
		private readonly HzzoDataProcessor _processor;

		public ProcessorV1Controller(HzzoDataProcessor processor)
		{
			this._processor = processor;
		}

		/// <summary>
		/// Starts the meds processor to scrape, download and parse remote .xls(x) documents from the specified Croatia's HZZO web domains.
		/// <para/>
		/// The processor can be re-executed by specifying the <paramref name="force"/> with a <c>True</c> value.
		/// <para/>
		/// Only a single (synchronized) execution of the processor is possible at one time, all other attempts will be enqueued. The following attempts will not invoke the processor but return the in-memory cached result.
		/// </summary>
		/// <param name="force">Specify if a processor re-execution will be enforeced.</param>
		/// <returns>Returns a JSON formatted message with the information about the processor run including info about processed documents and time of processing.</returns>
		[HttpGet("processor/run/{force?}")]
		public async Task<ActionResult<ApiMessageResponse>> GetRun([FromRoute] bool force = false) =>
			ApiResponse.ForMessage(await _processor.RunAsync(force));

		/// <summary>
		/// Gets information about the current state of the processor such as execution times and if the processor has successfully ran to completion.
		/// </summary>
		/// <returns>Returns a JSON formatted message informing about the status of the processor.</returns>
		[HttpGet("processor/status", Name = "v1.0_Processor_GetStatus")]
		public ActionResult<ApiDataResponse<HzzoDataProcessorStatus>> GetStatus() =>
			ApiResponse.ForData(_processor.GetStatus());

		/// <summary>
		/// Trys deleting the downloaded .xls(x) documents from the disk and clearing the in-memory dataset of loaded meds.
		/// <para/>
		/// The action can not be performed if processor is currently executing.
		/// <para/>
		/// The internal method is called by a force=true call to the action method: <see cref="ProcessorV1Controller.GetRun(bool)"/>.
		/// </summary>
		/// <returns>Returns a JSON formatted message informing if the clear action was successful.</returns>
		[HttpGet("processor/clear-data")]
		public ActionResult<ApiMessageResponse> GetClearData() =>
			ApiResponse.ForMessage(_processor.ClearData()
			? "Processor data cleared! You now must rerun the processor to continue using the API."
			: "Processor is running... Data can not be cleared during processing! Try again later.");
	}
}