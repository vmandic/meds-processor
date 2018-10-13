using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

		public string FilePath =>
			_rootLocation + FileName;

		public bool IsAlreadyDownloaded
			=> File.Exists(FilePath);

		public string Href { get; internal set; }
		public DateTime ValidFrom { get; private set; }
		public Stream DocumentStream { get; private set; }

		public async Task<HzzoMedsDownloadDto> TryDownloadDocument(HttpClient httpCli)
		{
			DocumentStream = !IsAlreadyDownloaded ?
				await httpCli.GetStreamAsync(this.Href) :
				null;

			return this;
		}
	}
}