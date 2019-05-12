using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MedsProcessor.Common;
using MedsProcessor.Common.Models;

namespace MedsProcessor.WebAPI.Core
{
	public class HzzoData
	{
		public HzzoData(AppPathsInfo appPaths)
		{
			this._appPaths = appPaths;
		}

		private ISet<HzzoMedsDownloadDto> _set;
		private readonly AppPathsInfo _appPaths;

		public ISet<HzzoMedsDownloadDto> Set
		{
			get => _set ??
				throw new InvalidOperationException("The dataset was not loaded.");

			private set
			{
				if (_set != null)
					throw new InvalidOperationException("The dataset was already loaded.");

				_set = value ??
					throw new InvalidOperationException("The loaded dataset can not be null.");
			}
		}

		internal void Load(ISet<HzzoMedsDownloadDto> data) =>
			Set = data;

		internal bool IsLoaded() =>
			_set != null && _set.All(x => x.IsDownloaded && x.IsDataParsed);

		internal void Clear()
		{
			_set = null;

			var downloadDir = Path.Combine(_appPaths.ApplicationRootPath, Constants.DOWNLOAD_DIR);
			Directory.Delete(downloadDir, true);
		}
	}
}