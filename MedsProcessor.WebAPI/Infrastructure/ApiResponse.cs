using System.Collections.Generic;
using System.Linq;
using MedsProcessor.Common;
using MedsProcessor.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Infrastructure
{
	public static class ApiResponse
	{
		public static ObjectResult ForCode(int statusCode) =>
			new ObjectResult(
				new Models.ApiHttpResponse(statusCode))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForMessage(string message, int statusCode = 200) =>
			new ObjectResult(
				new ApiMessageResponse(statusCode, message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForData<TObjectModel>(
				TObjectModel model,
				int statusCode = 200,
				string message = null) =>
			new ObjectResult(
				new ApiDataResponse<TObjectModel>(statusCode, model, message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForPagedData<TObjectModel>(
				IEnumerable<TObjectModel> model,
				int page,
				int size,
				int statusCode = 200,
				string message = null) =>
			new ObjectResult(
				new ApiPagedDataResponse<IEnumerable<TObjectModel>>(
					statusCode,
					GetPaged(page, size, model),
					page,
					size,
					message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult TryForPagedData<TObjectModel>(
				IEnumerable<TObjectModel> model,
				int? page = null,
				int? size = null,
				int statusCode = 200,
				string messageForPagedData = null) =>
			page.HasValue && size.HasValue
				? ForPagedData(
						model,
						page.Value,
						size.Value,
						statusCode,
						messageForPagedData)
				:	ForData(
						model,
						statusCode,
						message: "To get paged data sepcify both 'page' and 'size' query params.");

		private static IEnumerable<TObjectModel> GetPaged<TObjectModel>(
			int page,
			int size,
			IEnumerable<TObjectModel> model) =>
		model
			.Skip((page - 1) * size)
			.Take(size);
	}
}