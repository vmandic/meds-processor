using System.Text;
using MedsProcessor.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
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
      var statusUrl = Url.RouteUrl("Processor_GetStatus", new { apiVersion = "1.0" });

      var sb = new StringBuilder();

      sb.Append(isFirstTimeRun ?
          "The HZZO meds-processor has started with the Web API launch!" :
          "Welcome to the HZZO meds-processor.")
        .Append($" You can check the status at: { statusUrl }");

      isFirstTimeRun = false;

      return ApiResponse.ForMessage(sb.ToString());
    }
  }
}