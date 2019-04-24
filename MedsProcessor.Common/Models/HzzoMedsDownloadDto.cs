using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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

		public string FilePath =>
			Path.Combine(_rootLocation, FileName);

		public bool IsAlreadyDownloaded =>
			File.Exists(FilePath);

		public string FileName =>
			ValidFrom.ToString("yyyy-MM-dd_") +
			(Href.Split('/').LastOrDefault() ?? Href.Replace("/", "_").Replace(":", "_")).TrimEnd();

		public ISet<HzzoMedsImportDto> MedsList { get; } = new HashSet<HzzoMedsImportDto>();

		public string Href { get; set; }
		public DateTime ValidFrom { get; private set; }

		[IgnoreDataMember]
		public Task<Stream> DocumentStream { get; set; }
	}
}