using System;

namespace MedsProcessor.Common.Models
{
	public sealed class AppPathsInfo
	{
		public AppPathsInfo(string appRootPath)
		{
			if (appRootPath == null)
				throw new ArgumentNullException(nameof(appRootPath));

			ApplicationRootPath = appRootPath;
		}
		public string ApplicationRootPath { get; private set; }
	}
}