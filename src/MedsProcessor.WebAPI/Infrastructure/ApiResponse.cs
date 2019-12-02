using System.Collections.Generic;
using System.Linq;
using MedsProcessor.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedsProcessor.WebAPI.Infrastructure
{
	public static class ApiResponse
	{
			public static ObjectResult ForMessage(string message, int statusCode = 200) =>
			new ObjectResult(
				new ApiMessageResponse(message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForData<TObjectModel>(
				TObjectModel model,
				string message = null,
				int statusCode = 200) where TObjectModel : class =>
			new ObjectResult(
				new ApiDataResponse<TObjectModel>(model, message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult ForPagedData<TObjectModel>(
				IEnumerable<TObjectModel> model,
				int page,
				int size,
				string message = null,
				int statusCode = 200) =>
			new ObjectResult(
				new ApiPagedDataResponse<IEnumerable<TObjectModel>>(
					GetPaged(page, size, model),
					page,
					size,
					model.LongCount(),
					message))
			{
				StatusCode = statusCode
			};

		public static ObjectResult TryForPagedData<TObjectModel>(
				IEnumerable<TObjectModel> model,
				int? page = null,
				int? size = null,
				string messageForPagedData = null,
				int statusCode = 200) =>
			page.HasValue && size.HasValue ?
			ForPagedData(
				model,
				page.Value,
				size.Value,
				messageForPagedData,
				statusCode) :
			ForData(
				model,
				"To get paged data sepcify both 'page' and 'size' query params.",
				statusCode);

		private static IEnumerable<TObjectModel> GetPaged<TObjectModel>(
				int page,
				int size,
				IEnumerable<TObjectModel> model) =>
			model
			.Skip((page - 1) * size)
			.Take(size);
	}
}