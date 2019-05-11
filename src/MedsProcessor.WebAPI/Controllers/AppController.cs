using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
  [ApiController, Route("~/")]
  public class AppController : ControllerBase
  {
    public async Task<ActionResult> Index(
      [FromServices] HzzoHtmlScraper scraper, [FromServices] HzzoExcelDownloader downloader, [FromServices] HzzoExcelParser parser)
    {
      var startTime = DateTime.Now;

      var meds =
        parser.Run(
          await downloader.Run(
            await scraper.Run()));

      var totalTime = startTime - DateTime.Now;

      return Ok(
        $"Done! Handler duration: {totalTime.Duration()}" + Environment.NewLine +
        $"Documents ({meds.Count}) downloaded on path: '{downloader.DownloadDirPath}'" + Environment.NewLine +
        $"Total records parsed: {meds.SelectMany(x => x.MedsList).Count()}" + Environment.NewLine +
        Environment.NewLine +
        string.Join(Environment.NewLine, meds.Select(x => x.FileName))
      );
    }
  }
}