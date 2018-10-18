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
		readonly string _downloadDirPath;
		readonly IHttpClientFactory _httpCliFact;

		public HzzoExcelDownloader(IHttpClientFactory httpCliFact, AppPathsInfo appPathsInfo)
		{
			this._httpCliFact = httpCliFact;
			this._downloadDirPath = $@"{appPathsInfo.ApplicationRootPath}\{DOWNLOAD_DIR}";

			if (!Directory.Exists(_downloadDirPath))
				Directory.CreateDirectory(_downloadDirPath);
		}

		public async Task<ISet<HzzoMedsDownloadDto>> Run(ISet<HzzoMedsDownloadDto> meds)
		{
			// NOTE: throttle requests in parallel:
			var parallelismDegree = 5;
			var waitBetweenRequestsMs = 500;

			var httpCli = _httpCliFact.CreateClient();
			var savingItems = new List<Task>();
			var notDownloadedDocs = meds.Where(x => !x.IsAlreadyDownloaded).ToList();

			for (int i = 0; i < notDownloadedDocs.Count; i += parallelismDegree)
			{
				var queuedMeds = meds.Skip(i).Take(parallelismDegree);

				savingItems.AddRange(
					(await Task.WhenAll(
						queuedMeds.Select(med => med.DownloadExcel(httpCli))))
					.Select(SaveExcel));

				await Task.Delay(waitBetweenRequestsMs);
			}

			Task.WaitAll(savingItems.ToArray());

			return meds;
		}

		/// <summary>
		/// Copies the contents of input to output. Doesn't close either stream.
		/// </summary>
		static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[BUFFER_SIZE];
			int len;

			while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
				output.Write(buffer, 0, len);
		}

		static Task SaveExcel(HzzoMedsDownloadDto doc) =>
			Task.Factory.StartNew(() =>
			{
				using(var fileStream = File.Create($"{DOWNLOAD_DIR}{doc.FileName}", BUFFER_SIZE, FileOptions.Asynchronous))
				{
					CopyStream(doc.DocumentStream, fileStream);
				}
			}, TaskCreationOptions.LongRunning);
	}
}