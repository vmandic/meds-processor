using MedsProcessor.WebAPI.Models;

namespace MedsProcessor.WebAPI
{
	public static class ApiResponse
	{
		public static ApiHttpResponse ForCode(int statusCode) =>
			new ApiHttpResponse(statusCode);

		public static ApiMessageResponse ForMessage(int statusCode, string message) =>
			new ApiMessageResponse(statusCode, message);
		public static ApiMessageResponse ForMessageOk(string message) =>
			new ApiMessageResponse(200, message);

		public static ApiDataResponse<TObjectModel> ForData<TObjectModel>(int statusCode, TObjectModel model, string message = null) =>
			new ApiDataResponse<TObjectModel>(statusCode, model, message);
		public static ApiDataResponse<TObjectModel> ForDataOk<TObjectModel>(TObjectModel model, string message = null) =>
			new ApiDataResponse<TObjectModel>(200, model, message);
	}
}