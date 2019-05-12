using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;
using MedsProcessor.WebAPI.Core;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
  [ApiController, Route("~/")]
  public class AppController : ControllerBase
  {
		private readonly HzzoDataProcessor _processor;

		public AppController(HzzoDataProcessor processor)
		{
			this._processor = processor;
		}

    public async Task<ActionResult> Index(bool force = false)
    {
      return Ok(await _processor.Run(force));
    }
  }
}