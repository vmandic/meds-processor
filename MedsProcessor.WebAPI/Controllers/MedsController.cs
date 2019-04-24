using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Controllers
{
	[ApiController, Route("[controller]")]
	public class MedsController : ControllerBase
	{
		private readonly HzzoData _data;
		public MedsController(HzzoData data)
		{
			this._data = data;
		}

		[HttpGet("dump/{years:regex(^(\\d{{1,}},?)*$)?}")]
		public ActionResult DumpJson(string years = null)
		{
			if (!string.IsNullOrEmpty(years))
			{
				var filter = years.Split(",").Select(int.Parse);
				return Ok(_data.Set.Where(x => filter.Contains(x.ValidFrom.Year)));
			}

			return Ok(_data.Set);
		}

		
	}
}