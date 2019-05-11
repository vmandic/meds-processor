using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using static MedsProcessor.Common.Constants;

namespace MedsProcessor.Downloader
{
	public class HzzoExcelDownloader
	{
		const int BUFFER_SIZE = 1024;
		public string DownloadDirPath { get; }
		readonly HttpClient _httpCli;

		public HzzoExcelDownloader(IHttpClientFactory httpCliFact, AppPathsInfo appPathsInfo)
		{
			this._httpCli = httpCliFact.CreateClient();
			this.DownloadDirPath = Path.Combine(appPathsInfo.ApplicationRootPath, DOWNLOAD_DIR);
		}

		public async Task<ISet<HzzoMedsDownloadDto>> Run(ISet<HzzoMedsDownloadDto> meds, bool force = false)
		{
			if (!Directory.Exists(DownloadDirPath))
				Directory.CreateDirectory(DownloadDirPath);

			// NOTE: throttle requests in parallel
			var parallelismDegree = 5;
			var waitBetweenRequestsMs = 500;

			var notDownloadedCount = meds.Count(x => force || !x.IsDownloaded);

			for (int i = 0; i < notDownloadedCount; i += parallelismDegree)
			{
				var queuedMeds = meds.Skip(i).Take(parallelismDegree);
				await Task.WhenAll(queuedMeds.Select(DownloadExcel).Select(x => x.DocumentStream));
				await Task.WhenAll(queuedMeds.Select(SaveExcel));
				await Task.Delay(waitBetweenRequestsMs);
			}

			return meds;
		}

		HzzoMedsDownloadDto DownloadExcel(HzzoMedsDownloadDto doc)
		{
			doc.DocumentStream = _httpCli.GetStreamAsync(doc.Href);
			return doc;
		}

		Task SaveExcel(HzzoMedsDownloadDto doc) =>
			Task.Factory.StartNew(async() =>
			{
				using(var fileStream = File.Create(doc.FilePath, BUFFER_SIZE, FileOptions.Asynchronous))
				using(doc.DocumentStream)
				{
					await CopyStream(doc.DocumentStream.Result, fileStream);
					await fileStream.FlushAsync();
					await doc.DocumentStream.Result.FlushAsync();
				}
			}, TaskCreationOptions.LongRunning);

		/// <summary>
		/// Copies the contents of input to output. Doesn't close either stream.
		/// </summary>
		static async Task CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[BUFFER_SIZE];
			int len;

			while ((len = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
				await output.WriteAsync(buffer, 0, len);
		}
	}
}