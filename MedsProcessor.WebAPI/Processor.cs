using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;

namespace MedsProcessor.WebAPI
{
	public class Processor
	{
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
		private readonly HzzoHtmlScraper _scraper;
		private readonly HzzoExcelDownloader _downloader;
		private readonly HzzoExcelParser _parser;
		private readonly HzzoData _data;

		public Processor(
			HzzoHtmlScraper scraper,
			HzzoExcelDownloader downloader,
			HzzoExcelParser parser,
			HzzoData data)
		{
			this._downloader = downloader;
			this._parser = parser;
			this._data = data;
			this._scraper = scraper;
		}

		private string GetParseDetails() =>
			Environment.NewLine +
			$"Documents ({_data.Set.Count}) downloaded on path: '{_downloader.DownloadDirPath}'" +
			Environment.NewLine +
			$"Total records parsed: {_data.Set.SelectMany(x => x.MedsList).Count()}" + Environment.NewLine +
			Environment.NewLine +
			string.Join(Environment.NewLine, _data.Set.Select(x => x.FileName));

		public async Task<string> Run()
		{
			// utilize if-lock-if pattern
			if (!_data.IsLoaded())
			{
				try
				{
					await semaphoreSlim.WaitAsync();

					if (!_data.IsLoaded())
					{
						var startTime = DateTime.Now;

						var scraped = await _scraper.Run();
						var downloaded = await _downloader.Run(scraped);
						var meds = await _parser.Run(downloaded);

						_data.Set = meds;

						var totalTime = startTime - DateTime.Now;

						return $"Processed! Handler duration: {totalTime.Duration()}{GetParseDetails()}";
					}
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}

			return $"Processing skipped! Data was already loaded.{GetParseDetails()}";
		}
	}
}