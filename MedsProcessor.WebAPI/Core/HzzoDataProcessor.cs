using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedsProcessor.Common;
using MedsProcessor.Common.Models;
using MedsProcessor.Downloader;
using MedsProcessor.Parser;
using MedsProcessor.Scraper;

namespace MedsProcessor.WebAPI.Core
{
	public class HzzoDataProcessor
	{
		private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
		private TimeSpan? _lastRunDuration;
		private DateTime? _lastRunFinishedOn;
		private ProcessorState _state = ProcessorState.NotRan;
		private int _timesRan = 0;
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
			$"Documents: {string.Join(Environment.NewLine, _data.Set.Select(x => x.FileName))}";

		public async Task<string> Run(bool force = false)
		{
			// utilize if-lock-if pattern
			if (force || !_data.IsLoaded())
			{
				try
				{
					await _semaphoreSlim.WaitAsync();

					if (force || !_data.IsLoaded())
					{
						if (force)
						{
							ClearData();
						}

						_state = ProcessorState.Running;
						var startTime = DateTime.Now;

						var scrapedHtml = await _scraper.Run();
						var downloadedXls = await _downloader.Run(scrapedHtml, force);
						var parsedMeds = _parser.Run(downloadedXls);

						_data.Load(parsedMeds);
						_state = ProcessorState.Ran;
						_timesRan++;
						_lastRunDuration = DateTime.Now - startTime;
						_lastRunFinishedOn = DateTime.Now;

						return $"Processed! Handler duration: {_lastRunDuration}{GetParseDetails()}";
					}
				}
				finally
				{
					_semaphoreSlim.Release();
				}
			}

			return $"Skipped! Data already processed on: {_lastRunFinishedOn} (Duration was: {_lastRunDuration}){GetParseDetails()}";
		}

		public HzzoDataProcessorStatus GetStatus() =>
			new HzzoDataProcessorStatus(
				_data.IsLoaded(),
				_state,
				_timesRan,
				_lastRunFinishedOn,
				_lastRunDuration);

		public bool ClearData()
		{
			if (_state != ProcessorState.Running)
			{
				_data.Clear();
				_state = ProcessorState.DataCleared;

				return true;
			}

			return false;
		}
	}
}