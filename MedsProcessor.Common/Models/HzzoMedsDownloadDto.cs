using System;
using System.Linq;

namespace MedsProcessor.Common.Models
{
	public class HzzoMedsDownloadDto
	{
		private readonly string _rootLocation;

		public HzzoMedsDownloadDto(string href, string validFrom, string rootLocation)
		{
			this.Href = href;
			this.ValidFrom = DateTime.Parse(validFrom);
			this._rootLocation = rootLocation;
		}

		public string FileName =>
			ValidFrom.ToString("yyyy-MM-dd_") +
			(Href.Split('/').LastOrDefault() ?? Href.Replace("/", "_").Replace(":", "_")).TrimEnd();

		public string Href { get; internal set; }
		public DateTime ValidFrom { get; private set; }
	}
}