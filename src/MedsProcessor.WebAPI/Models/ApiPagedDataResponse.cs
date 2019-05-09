using System;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiPagedDataResponse<TObjectModel> : ApiDataResponse<TObjectModel>
	{
		public ApiPagedDataResponse(
			int statusCode,
			TObjectModel model,
			int pageNumber,
			int pageSize,
			string message = null) : base(statusCode, model, message)
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
		}

		public int PageNumber { get; }
		public int PageSize { get; }
	}
}