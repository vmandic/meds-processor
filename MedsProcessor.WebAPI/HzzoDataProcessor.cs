using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;

namespace MedsProcessor.WebAPI
{
	public class HzzoDataProcessor
	{
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
		private static TimeSpan totalTime;
		private static DateTime lastProcessingFinishedOn;
		private readonly HzzoHtmlScraper _scraper;
		private readonly HzzoExcelDownloader _downloader;
		private readonly HzzoExcelParser _parser;
		private readonly HzzoData _data;

		public HzzoDataProcessor(
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
			$"Documents ({_data.Set.Count}) are downloaded on path: '{_downloader.DownloadDirPath}'" +
			Environment.NewLine +
			$"Total records parsed: {_data.Set.SelectMany(x => x.MedsList).Count()}" +
			Environment.NewLine +
			Environment.NewLine +
			string.Join(Environment.NewLine, _data.Set.Select(x => x.FileName));

		public async Task<string> Run(bool force = false)
		{
			// utilize if-lock-if pattern
			if (force || !_data.IsLoaded())
			{
				try
				{
					await semaphoreSlim.WaitAsync();

					if (force || !_data.IsLoaded())
					{
						var startTime = DateTime.Now;

						var scrapedHtml = await _scraper.Run();
						var downloadedXls = await _downloader.Run(scrapedHtml, force);
						var parsedMeds = _parser.Run(downloadedXls);

						_data.Load(parsedMeds, force);

						totalTime = startTime - DateTime.Now;
						lastProcessingFinishedOn = DateTime.Now;
						return $"Processed! Handler duration: {totalTime.Duration()}{GetParseDetails()}";
					}
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}

			return $"Skipped! Data already processed on: {lastProcessingFinishedOn} (Duration was: {totalTime.Duration()}){GetParseDetails()}";
		}
	}
}