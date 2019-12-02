using System;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiPagedDataResponse<TObjectModel> : ApiDataResponse<TObjectModel>
	{
		public ApiPagedDataResponse(
			TObjectModel model,
			int pageNumber,
			int pageSize,
			long totalItems,
			string message = null) : base(model, message)
		{
			if (pageNumber < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(pageNumber));
			}

			if (pageSize < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(pageSize));
			}

			PageNumber = pageNumber;
			PageSize = pageSize;
			TotalItems = totalItems;
		}

		public int PageNumber { get; }
		public int PageSize { get; }
		public long TotalItems { get; }
	}
}