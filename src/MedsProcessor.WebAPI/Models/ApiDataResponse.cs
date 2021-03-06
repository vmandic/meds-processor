using System;

namespace MedsProcessor.WebAPI.Models
{
	public class ApiDataResponse<TObjectModel> : ApiMessageResponse where TObjectModel : class
	{
		public TObjectModel Data { get; }

		public ApiDataResponse(TObjectModel model, string message = null) : base(message)
		{
			if (model == default(TObjectModel))
			{
				throw new ArgumentNullException("Response object model data can not be null or of default value for the given model data.");
			}

			Data = model;
		}
	}
}