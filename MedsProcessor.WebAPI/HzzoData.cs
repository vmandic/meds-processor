using System;
using System.Collections.Generic;
using MedsProcessor.Common.Models;

namespace MedsProcessor.WebAPI
{
	public class HzzoData
	{
		private ISet<HzzoMedsDownloadDto> _set;
		public ISet<HzzoMedsDownloadDto> Set
		{
			get => _set ??
				throw new InvalidOperationException("The dataset was not loaded.");

			set
			{
				if (_set != null)
					throw new InvalidOperationException("The dataset was already loaded.");

				_set = value ??
					throw new InvalidOperationException("The loaded set can not be null.");
			}
		}

		public bool IsLoaded() => _set != null;
	}
}