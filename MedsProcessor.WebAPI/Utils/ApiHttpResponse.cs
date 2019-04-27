using MedsProcessor.Common;
using MedsProcessor.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI
{
	public static class ApiHttpResponse
	{
		public static ObjectResult ForCode(int statusCode) =>
			new ObjectResult(new Models.ApiHttpResponse(statusCode))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForMessage(string message, int statusCode = 200) =>
			new ObjectResult(new ApiMessageResponse(statusCode, message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForData<TObjectModel>(
				TObjectModel model,
				int statusCode = 200,
				string message = null) =>
			new ObjectResult(new ApiDataResponse<TObjectModel>(statusCode, model, message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForPage<TObjectModel>(
				TObjectModel model,
				int? page = null,
				int? size = null,
				int statusCode = 200,
				string message = null) =>
			new ObjectResult(new ApiPageResponse<TObjectModel>(
				statusCode,
				model,
				page ?? Constants.PAGE_NUMBER,
				size ?? Constants.PAGE_SIZE,
				message))
				{
					StatusCode = statusCode
				};
	}
}