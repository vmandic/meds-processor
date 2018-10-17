using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MedsProcessor.Common.Models;
using static MedsProcessor.Common.Constants;

namespace MedsProcessor.Downloader
{
	public class HzzoExcelDownloader
	{
		readonly string _downloadDirPath;
		readonly IHttpClientFactory _httpCliFact;

		public HzzoExcelDownloader(IHttpClientFactory httpCliFact, AppPathsInfo appPathsInfo)
		{
			this._httpCliFact = httpCliFact;
			this._downloadDirPath = $@"{DOWNLOAD_DIR}\{appPathsInfo.ApplicationRootPath}";

			if (!Directory.Exists(_downloadDirPath))
				Directory.CreateDirectory(_downloadDirPath);
		}

		public async Task Run(ISet<HzzoMedsDownloadDto> meds)
		{
			// TODO: implement
		}
	}
}